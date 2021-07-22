$(document).ready(function () {

    var storedRepositories = null;
    loadExistingRepositories();

    function loadExistingRepositories() {
        $.ajax({
            method: "POST",
            url: "/repository",
            success: function (response) {
                storedRepositories = response["Repositories"];
                $.each(storedRepositories, function (index, repository) {
                    $('#repository').append($('<option/>', {
                        value: index,
                        text: repository.Name
                    }))
                });
            },
            error: function (response) {
                console.log("error:" + response);
            }
        });
    }

    $('#repository').on('change', function () {
        if (this.value == "default") {
            $('#link').removeAttr("disabled");
            $('#name').removeAttr("disabled");
            $('#link').val('');
            $('#name').val('');
            ClearBranchSelect();
            return;
        }

        $('#link').attr("disabled", "disabled");
        $('#name').attr("disabled", "disabled");
        var selectedRepo = storedRepositories[this.value];

        $('#link').val(selectedRepo.Url);
        $('#name').val(selectedRepo.Name);
        ClearBranchSelect();
        GetListOfBranches(selectedRepo.Url);
    });


    function GetListOfBranches(repositoryURL) {
        var url = new URL(repositoryURL);
        var path = url.pathname.split('/');
        SuggestProjectName(path[2]);

        if (url.hostname.match("github.com")) {
            CallGitHubAPI(path[1], path[2]);
        } else if (url.hostname.match("bitbucket.org")) {
            CallBitbucketAPI(path[1], path[2]);
        }

    }

    function SuggestProjectName(repositoryName) {
        var projectNameField = $('#name');
        if (!projectNameField.val()) {
            projectNameField.val(repositoryName);
        }
    }

    function CallBitbucketAPI(repository, workspace) {
        var apiLink = "https://api.bitbucket.org/2.0/repositories/" + repository + "/" + workspace + "/refs/branches";
        $.ajax({
            method: "GET",
            url: apiLink,
            dataType: "json",
            success: function (response) {
                var data = response.values.map(branch => branch.name);
                if (data !== null) {
                    var branchSelect = $('#branch');
                    branchSelect.removeAttr("disabled");
                    data.forEach(function (branch) {
                        branchSelect.append($('<option>', {
                            value: branch,
                            text: branch
                        }));
                    });
                }
            },
            error: function (response) {
                Swal.fire(
                  "Nie można uzyskać dostępu do repozytorium",
                  "Proszę sprawdzić ustawienia dostępności repozytorium.",
                  'error'
                );
            }
        });
    }

    function CallGitHubAPI(user, repository) {
        var apiLink = "https://api.github.com/repos/" + user + "/" + repository + "/branches";
        $.ajax({
            method: "GET",
            url: apiLink,
            dataType: "json",
            success: function (response) {
                var data = response.map(branch => branch.name);
                if (data !== null) {
                    var branchSelect = $('#branch');
                    branchSelect.removeAttr("disabled");
                    data.forEach(function (branch) {
                        branchSelect.append($('<option>', {
                            value: branch,
                            text: branch
                        }));
                    });
                }
            },
            error: function (response) {
                Swal.fire(
                  "Nie można uzyskać dostępu do repozytorium",
                  "Proszę sprawdzić ustawienia dostępności repozytorium.",
                  'error'
                );
            }
        });
    }

    function ClearBranchSelect() {
        var branchSelect = $('#branch');
        branchSelect.prop("disabled", true);
        branchSelect.find('option').remove().end();
    }


    $('#link').change(function () {
        var linkField = $('#link');
        linkField.removeClass('error').next($('#link-error')).remove();
        var validator = linkField.validate({
            rules: {
                link: {
                    required: true,
                    url: true,
                    minLength: 5
                },
            },
            messages: {
              link: "Proszę wprowadzić link do repozytorium",
            },
            errorClass: "text-danger",
            success: function (label, element) {
                ClearBranchSelect();
                GetListOfBranches(element.value);
            }
        });

        validator.element("#link");
    });


    $('#form').validate({
        rules: {
            name: {required: true},
            link: {
                required: true,
                url: true,
                minLength: 5
            },
            branch: {required: true}
        },
        messages: {
          name: "Proszę wprowadzić nazwę projektu",
          link: "Proszę wprowadzić link do repozytorium",
        },
        errorClass: "text-danger",

        submitHandler: function (form) {
          let apiLink = "/analysis/analysis";
          let name = $('#name').val();
          let url = $('#link').val();
          let branch = $('#branch').val();
          let branchId = ($('#repository').val() === "default") ? null : storedRepositories[$('#repository').val()].BranchId;
          let repositoryId = ($('#repository').val() === "default") ? null : storedRepositories[$('#repository').val()].RepositoryId;
          if (repositoryId != null) apiLink = "/analysis/analysisupdate";
          console.log(apiLink);
          $.ajax({
            method: "POST",
            url: apiLink,
            data: {
              name: name,
              url: url,
              branch: branch,
              branchId: branchId,
              repositoryId: repositoryId
            },
                datatype: JSON,
                beforeSend: function () {
                    Swal.fire({
                        imageUrl: "/images/loading_circle.svg",
                        showConfirmButton: false,
                      text: "Trwa analiza projektu, która może potrwać kilka minut"
                    });
                },
                success: function (response) {
                    Swal.fire(
                      "Projekt dodany",
                      "Trwa analiza projektu, która może potrwać kilka minut",
                      'success'
                    );
                },
                error: function (response) {
                    Swal.fire(
                      "Analiza nie została ukończona z powodu błędu",
                      "Proszę spróbować ponownie. W przypadku powracającego błędu proszę o kontakt z twórcą" +
                      " oprogramowania.",
                      'error'
                    );
                }
            });
            return false;
        }
    });
});
