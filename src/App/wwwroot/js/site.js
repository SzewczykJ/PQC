$(document).ready(function () {
  let stored_JSON;
  let repositoryInfo = null;
  let metrics;
  let commitDetails = null;
  let selectedRepository = $('#repository');
  let showCommitDetails = false;
  var $submitbutton = $("#submitbutton");
  var $updatebutton = $("#update");
  var $newbranchbutton = $("#new_branch");

  loadJSON(function (response) {
    metrics = JSON.parse(response);
  });

  function BuildChart(dataset, selectedMetric) {

    var gradientSteps = () => {
      const data = dataset.filter(commit => commit.Metrics != null && commit.Metrics[selectedMetric] != null)
        .map(commit => commit.Metrics[selectedMetric], 0);
      var maxValue = Math.max(...data);
      var metricData = metrics[selectedMetric];
      var totalSteps = metricData.valueLevel.length;
      var stops = [];

      $.each(Object.keys(metricData.valueLevel), function (index) {
        if (index === 0 || index === (totalSteps - 1)) {
          stops.push([metricData.valueLevel[index], metricData.colors[index]]);
        } else {
          stops.push([calculateGradientStep(maxValue, metricData.valueLevel[index]), metricData.colors[index]]);
        }
      });

      return stops;
    };

    metrics[selectedMetric].Chart = Highcharts.chart(selectedMetric + 'container', {
      chart: {
        type: 'area',
        zoomType: 'x',
      },
      title: {
        text: null
      },
      xAxis: {
        type: 'datetime',
        categories: dataset.map(el => new Date(el.Date).toLocaleDateString("en-GB")),
        rotate: -45,
        scrollbar: {
          enabled: true
        },
      },
      plotOptions: {
        series: {
          marker: {
            fillColor: '#FFFFFF',
            lineWidth: 2,
            radius: 4,
            lineColor: null // inherit from series
          },
          tooltip: {
            valueDecimals: 2
          }
        },

      },
      series: [{
        name: 'commit',
        data: dataset.filter(commit => commit.Metrics != null && commit.Metrics[selectedMetric] != null).map(el => el.Metrics[selectedMetric]),
        color: {
          linearGradient: [0, 250, 0, 0],
          stops: gradientSteps(),
        },

      }]
    });
  }

  let AddCanvasToPage = function (metric) {
    $('#graphs').append(
      $('<div/>', {
        class: 'row justify-content-center my-5'
      }).append(
        $('<div/>', {
          class: 'col-12 mb-4'
        }).append(
          $('<p/>').append(
            $('<button/>',
              {
                class: "btn btn-primary",
                text: metrics[metric].displayName,
                type: "button",
                "data-toggle": "collapse",
                "data-target": '#' + metric + "Description",
                "aria-expanded": false
              }).append($('<span/>',
              {
                class: "fas fa-angle-down js-rotate-if-collapsed ",
                "aria-hidden": true
              }
            ))
          ),
          $('<div/>', {
            class: "collapse mt-2",
            "data-toggle": "false",
            id: metric + "Description"
          }).append(
            $('<div/>', {
              class: "card card-body",
              html: metrics[metric].description
            })
          )
        ),
        $('<div/>', {
          id: metric + 'container',
          width: "100%",
          height: "400px",
        }),
        $('<div/>', {
          class: 'row justify-content-center my-5 col-12 p-0 overflow-auto',
          style: "max-height:400px !important"
        }).append(
          $('<div/>', {
            id: metric + "Details",
            class: "details col-12 p-0 d-none"
          })
        )
      )
    );


  };

  function GenerateGraphs() {
    $.each(Object.keys(metrics), function (index, metric) {
      // remove old charts
      if ($('#' + metric).length !== 0) {
        $('#' + metric).parent().parent().parent().parent().remove();
      }
      if (stored_JSON.CommitList.length === 0) return;
      AddCanvasToPage(metric);
      BuildChart(stored_JSON.CommitList, metric);
    });
  }


  //Switch send button style
  selectedRepository.on('change', function () {
    repositoryInfo = null;
    stored_JSON = null;
    if (selectedRepository.val() != false) {
      activateButton($submitbutton);
    } else {
      deactivateButton($submitbutton);
      deactivateButton($updatebutton);
      deactivateButton($newbranchbutton);

    }
  });

  // The function which calls endpoint for update existing analysis
  $('button[name="update"]').off('click').click(function (e) {
    e.preventDefault();
    if (repositoryInfo === null) return;
    var alertField = $('#analysisRequestAlert');
    alertField.removeClass('d-none');
    alertField.html("Trwa uaktualnienie analizy");
    $.ajax({
      method: "POST",
      url: "/analysis/analysisupdate",
      data: repositoryInfo,
      success: function (response) {
        alertField.removeClass("alert-info");
        alertField.addClass("alert-success");
        alertField.html("Aktualizacja analizy projektu, zakończona sukcesem");
        alertField.delay(5000).addClass('d-none');
      },
      error: function (response) {
        alertField.removeClass("alert-info");
        alertField.addClass("alert-warning");
        alertField.html("Aktualizacja analizy projektu, zakończona niepowodzeniem");
        alertField.delay(5000).addClass('d-none');
        console.log("error:" + response);
      }
    });

  });

  // load repository data with metrics value
  $('button[name="repository_confirm"]').click(function () {
    let selectedRepository = $('#repository').val();
    let selectedBranch = $('#branches').val();

    if (selectedRepository == false || selectedBranch == false) return;
    $.ajax({
      method: "POST",
      url: "/result/getresult",
      data: {
        RepositoryId: selectedRepository,
        BranchId: selectedBranch
      },
      success: function (response) {
        $('#chartImage').hide();
        $('#chartImageError').hide();

        stored_JSON = response["CommitSummary"];
        repositoryInfo = response["RespositoryInfo"];

        let data = stored_JSON.CommitList
          .filter(commit => commit.Metrics != null)
          .map(commit => commit)
          .sort((a, b) => a.Date > b.Date && 1 || -1);
        stored_JSON.CommitList = data;

        //activate update button
        activateButton($updatebutton);
        activateButton($newbranchbutton);
        $('#details').removeAttr("disabled");

        GenerateGraphs();
        $('.collapse').collapse('hide');

        if (stored_JSON.CommitList.length === 0) {
          $('#chartImageError').show();
        }
      },
      error: function (response) {
        console.log(response);
      }
    });
  });

  $('#details').click(function () {
    //verify state before toggle
    if ($(this).attr('aria-pressed') === 'false') {
      showCommitDetails = true;
      $('.details').removeClass("d-none");
    } else {
      showCommitDetails = false;
      $('.details').addClass("d-none");
    }
  });

  function calculateGradientStep(oldMax, value) {
    return (1 / oldMax) * (value - oldMax) + 1;
  }

  function activateButton(button) {
    button.removeAttr("disabled");
    button.removeClass("btn-secondary");
    button.addClass("btn-primary");
  }

  function deactivateButton(button) {
    button.attr("disabled", "disabled");
    button.addClass("btn-secondary");
    button.removeClass("btn-primary");
  }

  // load metrics descriptions with recommended values to generate gradient on graph
  function loadJSON(callback) {

    var xobj = new XMLHttpRequest();
    xobj.overrideMimeType("application/json");
    xobj.open('GET', 'js/MetricsInfo.json', true);
    xobj.onreadystatechange = function () {
      if (xobj.readyState == 4 && xobj.status == "200") {
        callback(xobj.responseText);
      }
    };
    xobj.send(null);
  }


  function loadCommitDetails(commitId) {
    $.ajax({
      method: "POST",
      url: "/result/getdetails",
      data: {
        commitId: commitId
      },
      success: function (response) {
        commitDetails = response;
        showCommitDetailsUnderGraph(commitId);
      },
      error: function (response) {
        console.error(response);
      }
    });
  }

  function showCommitDetailsUnderGraph(commitId) {
    $.each(Object.keys(metrics), function (index, metric) {
      var detailsBox = $('#' + metric + "Details");

      if (detailsBox.length) {
        detailsBox.html("");
        var table = $('<table>', {class: "table"}).appendTo(detailsBox);

        $.each(commitDetails["FileList"], function (index, json) {
            var rowColor = "table-default";

            if (isFinite(metrics[metric]["alertValue"])) {
              if (json.Metric[metric] > metrics[metric]["alertValue"]) {
                rowColor = "table-warning";
              }
            }
            if (isFinite(metrics[metric]["errorValue"])) {
              if (json.Metric[metric] > metrics[metric]["alertValue"]) {
                rowColor = "table-danger";
              }
            }


            var actualFileMetricValue = json.Metric[metric];
            if (actualFileMetricValue == null) actualFileMetricValue = "---";

            table.append(
              $('<tr/>').append(
                $("<th class=\"" + rowColor + " col-11 \">" + json.FullPath + "</th>"),
                $("<th class=\"" + rowColor + " col-1 \">" + actualFileMetricValue + "</th>")
              )
            );
          }
        );
      }
    });

  }


});
