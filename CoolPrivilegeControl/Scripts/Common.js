var LoadingDialog = new BootstrapDialog({
    type: BootstrapDialog.TYPE_INFO,
    message: "<div style=\"text-align:center;\"><i class=\"fa fa-refresh fa-spin fa-3x\" style=\"-webkit-animation: spin 1000ms infinite linear;animation: spin 1000ms infinite linear;\"></i></div>",
    closable: false
});

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

function InvokeDeleteFunction_ByAjax(json_Data, str_PostUrl, str_DetailsUrl) {
    OpenLoadingDialog();
    $.ajax({
        type: "post",
        dataType: "json",
        url: str_PostUrl,
        data: AddAntiForgeryToken(json_Data),
        success: function (response) {
            if (response != null && response != undefined) {
                if (response.Success) {
                    //ShowMsg(BootstrapDialog.TYPE_INFO, response.MessageTitle, response.Message);
                    document.location.href = response.ReturnUrl;
                }
                else {
                    //ShowMsg(BootstrapDialog.TYPE_DANGER, response.MessageTitle, response.Message);
                    document.location.href = response.ReturnUrl;
                }
            }
            else {
                //ShowMsg(BootstrapDialog.TYPE_DANGER, "Error Message", "Delete Failed");
                document.location.href = str_DetailsUrl;
            }
            CloseLoadingDialog();
        }
    });
}

function ShowMsg(DialogType, strTitle, strMsg) {
    BootstrapDialog.show({
        type: DialogType,
        title: strTitle,
        message: strMsg,
        closable: false,
        buttons: [{
            label: 'OK',
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
}

function findSortingTh(sort_Default, sortDir_Default, sortKey, sortDirKey) {
    if (sortKey == undefined || sortKey == "" || sortKey == null) {
        sortKey = "sort";
    }

    if (sortDirKey == undefined || sortDirKey == "" || sortDirKey == null) {
        sortDirKey = "sortDir";
    }
    if ($.url("?" + sortKey) != undefined && $.url("?" + sortKey) != null && $.url("?" + sortKey) != "") {
        var sortParm = $.url("?" + sortKey);
        var sortingth = $("th[id='" + sortParm + "TH']");

        if (sortingth != undefined && sortingth != null) {
            if ($.url("?" + sortDirKey) != undefined && $.url("?" + sortDirKey) != null && $.url("?" + sortDirKey) != "") {
                var originalIcon = sortingth.find("i[class^='fa fa-fw']");
                originalIcon.remove();
                sortingth.append("<i class=\"fa fa-fw fa-sort-" + $.url("?" + sortDirKey) + "\"></i>");
            }
        }
    }
    else {
        if (sort_Default != undefined && sort_Default != "" && sort_Default != null && sortDir_Default != undefined && sortDir_Default != "" && sortDir_Default != null) {
            var sortingth = $("th[id='" + sort_Default + "']");

            if (sortingth != null && sortingth != "" && sortingth != undefined) {
                sortingth.append("<i class=\"fa fa-fw fa-sort-" + sortDir_Default.toLowerCase() + "\"></i>");
            }
        }
    }
}

function sort(sender, sort, sortKey, sortDirKey) {
    if (sortKey == undefined || sortKey == "" || sortKey == null) {
        sortKey = "sort";
    }

    if (sortDirKey == undefined || sortDirKey == "" || sortDirKey == null) {
        sortDirKey = "sortDir";
    }

    var sortDir = "asc";
    var asc = $(sender).find("i[class^='fa fa-fw']");
    if (asc != null && asc != undefined && asc.length != 0) {
        if (asc.attr("class") == "fa fa-fw fa-sort-asc") {
            sortDir = "desc";
        }
        else {
            sortDir = "asc";
        }
    }

    var parms = $.url("?");

    $("th").each(function (index, value) {
        var originalIcon = $(value).find("i[class^='fa fa-fw']");
        if (originalIcon != null && originalIcon != undefined) {
            originalIcon.remove();
        }
    });

    if ($.url("?" + sortKey) != "") {
        var sortValue = $.url("?" + sortKey);
        parms = parms.replace("&" + sortKey + "=" + sortValue, "");
        parms = parms.replace(sortKey + "=" + sortValue, "");
    }

    if ($.url("?" + sortDirKey) != "") {
        var sortDirValue = $.url("?" + sortDirKey);
        parms = parms.replace("&" + sortDirKey + "=" + sortDirValue, "");
    }
    if (parms == "") {
        parms = sortKey + "=" + sort + "&" + sortDirKey + "=" + sortDir;
    }
    else {
        parms = parms + "&" + sortKey + "=" + sort + "&" + sortDirKey + "=" + sortDir;
    }

    var hrefPath = $.url("path");
    hrefPath = hrefPath + "?" + parms;
    $(location).attr('href', hrefPath);
}

function OpenLoadingDialog()
{
    LoadingDialog.realize();
    LoadingDialog.getModalHeader().hide();
    LoadingDialog.getModalFooter().hide();

    LoadingDialog.open();
}

function CloseLoadingDialog() {
    LoadingDialog.close();
}

function ExportResult(json_Data, str_GetUrl) {
    OpenLoadingDialog();
    var queryStr = $.url("?");
    $.ajax({
        type: "post",
        dataType: "json",
        url: str_GetUrl,
        data: AddAntiForgeryToken(json_Data),
        success: function (response) {
            if (response != null && response != undefined) {
                if (response.Success) {

                    var queryString = new Array();

                    if (response.OutputFileName != null && response.OutputFileName != undefined && response.OutputFileName != "") {
                        queryString.push("OutputFileName=" + response.OutputFileName);
                    }

                    if (response.Key != null && response.Key != undefined && response.Key != "") {
                        queryString.push("Key=" + response.Key);
                    }

                    if (response.FilePath != null && response.FilePath != undefined && response.FilePath != "") {
                        queryString.push("FilePath=" + response.FilePath);
                    }

                    DownloadFile(response.ReturnUrl + "?" + queryString.join("&"));
                }
                else {
                    document.location.href = response.ReturnUrl + "?" + queryStr;
                }
            }
            else {
                document.location.href = response.ReturnUrl + "?" + queryStr;
            }
            CloseLoadingDialog();
        }
    });
}

function DownloadFile(url) {
    $("#exportIframe").attr("src", url);
}