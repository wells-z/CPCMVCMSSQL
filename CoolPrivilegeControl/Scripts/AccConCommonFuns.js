/*------------------Organization Functions Start---------------------------*/
function LoadOrgList(IDStrList, OrgDetailsIDStrList, pageIndex, sort, sortDir) {
    var orgListIDList = "";
    var orgDetailsListIDList = "";
    var orgListPage = "1";
    var orgListSort = "";
    var orgListSortDir = "";

    if (IDStrList != "" && IDStrList != undefined) {
        orgListIDList = IDStrList;
    }

    if (OrgDetailsIDStrList != "" && OrgDetailsIDStrList != undefined) {
        orgDetailsListIDList = OrgDetailsIDStrList;
    }

    if (pageIndex != "" && pageIndex != undefined) {
        orgListPage = pageIndex;
    }

    if (sort != "" && sort != undefined) {
        orgListSort = sort;
    }

    if (sortDir != "" && sortDir != undefined) {
        orgListSortDir = sortDir;
    }

    var urlPath = $('#loader_OrgList').data("request-url");

    var json_Data = { orgListIDList: "" + orgListIDList + "", orgDetailsListIDList: "" + orgDetailsListIDList + "", orgListPage: "" + orgListPage + "", orgListSort: "" + orgListSort + "", orgListSortDir: "" + orgListSortDir + "" };
    $.ajax({
        type: "post",
        url: urlPath,
        data: AddAntiForgeryToken(json_Data),
        success: function (response) {
            if (response != null && response != undefined) {
                $("#OrgList").html(response);

                var UserType = $('input[name=UserType]').filter(':checked').val();

                if (UserType == 2) {
                    $("#FDContent").hide();
                    $("#RoleContent").show();
                    $("#OrgContent").hide();
                }
                else if (UserType == 1) {
                    $("#FDContent").show();
                    $("#RoleContent").hide();
                    $("#OrgContent").hide();
                }
                else if (UserType == 3) {
                    $("#FDContent").hide();
                    $("#RoleContent").hide();
                    $("#OrgContent").show();
                }
                else {
                    $("#FDContent").hide();
                    $("#RoleContent").hide();
                    $("#OrgContent").hide();
                }

            }
            else {
                $("#OrgList").empty();
            }
        }
    });
}

function AddOrg() {
    var orgListIDList = $("#orgListIDList").val();
    var orgDetailsIDList = $("#orgDetailsIDList").val();
    var orgListPage = $("#orgListPage").val();
    var orgListSort = $("#orgListSort").val();
    var orgListSortDir = $("#orgListSortDir").val();

    var orgArray = orgListIDList.split("|");

    var orgDetailsArray = orgDetailsIDList.split("|");

    var orgID_Selected = "";
    if ($("#OrgID").find("option:selected").length > 0) {
        orgID_Selected = $("#OrgID").find("option:selected").val();
    }

    var orgDetailsID_Selected = "";
    if ($("#OrgDetailsID").find("option:selected").length > 0) {
        orgDetailsID_Selected = $("#OrgDetailsID").find("option:selected").val();
    }

    if (orgID_Selected != "" && orgDetailsID_Selected != "") {
        var isExists = false;

        if (orgID_Selected != "" && orgDetailsID_Selected != "") {
            for (var i = 0; i < orgArray.length; ++i) {
                if (orgArray[i] == orgID_Selected && orgDetailsArray[i] == orgDetailsID_Selected) {
                    isExists = true;
                }
            }
        }

        if (!isExists) {
            if (orgListIDList == "") {
                orgListIDList = orgID_Selected;
            }
            else {
                orgListIDList = orgListIDList + "|" + orgID_Selected;
            }

            if (orgDetailsIDList == "") {
                orgDetailsIDList = orgDetailsID_Selected;
            }
            else {
                orgDetailsIDList = orgDetailsIDList + "|" + orgDetailsID_Selected;
            }
        }

        $("#orgListIDList").val(orgListIDList);

        $("#orgDetailsIDList").val(orgDetailsIDList);

        LoadOrgList(orgListIDList, orgDetailsIDList, orgListPage, orgListSort, orgListSortDir);
    }
}

function DeleteOrg(orgID, orgDetailsID) {
    var orgListIDList = $("#orgListIDList").val();
    var orgDetailsIDList = $("#orgDetailsIDList").val();
    var orgListPage = "1";
    var orgListSort = $("#orgListSort").val();
    var orgListSortDir = $("#orgListSortDir").val();

    var orgArray = orgListIDList.split("|");

    var orgDetailsArray = orgDetailsIDList.split("|");

    var orgListIDList_New = "";

    var orgDetailsListIDList_New = "";

    for (var i = 0; i < orgArray.length; ++i) {
        if (orgArray[i] != orgID || orgDetailsArray[i] != orgDetailsID) {
            if (orgListIDList_New == "") {
                orgListIDList_New = orgArray[i];
            }
            else {
                orgListIDList_New += "|" + orgArray[i];
            }

            if (orgDetailsListIDList_New == "") {
                orgDetailsListIDList_New = orgDetailsArray[i];
            }
            else {
                orgDetailsListIDList_New += "|" + orgDetailsArray[i];
            }
        }
    }

    $("#orgListIDList").val(orgListIDList_New);

    $("#orgDetailsIDList").val(orgDetailsListIDList_New);

    LoadOrgList(orgListIDList_New, orgDetailsListIDList_New, orgListSort, orgListSortDir);
}
/*------------------Organization Functions End---------------------------*/

/*------------------Role Functions Start---------------------------*/
function LoadRoleList(IDStrList, pageIndex, sort, sortDir) {
    var roleListIDList = "";
    var roleListPage = "1";
    var roleListSort = "";
    var roleListSortDir = "";

    if (IDStrList != "" && IDStrList != undefined) {
        roleListIDList = IDStrList;
    }

    if (pageIndex != "" && pageIndex != undefined) {
        roleListPage = pageIndex;
    }

    if (sort != "" && sort != undefined) {
        roleListSort = sort;
    }

    if (sortDir != "" && sortDir != c) {
        roleListSortDir = sortDir;
    }

    var urlPath = $('#loader_RoleList').data("request-url");

    var json_Data = { roleListIDList: "" + roleListIDList + "", roleListPage: "" + roleListPage + "", roleListSort: "" + roleListSort + "", roleListSortDir: "" + roleListSortDir + "" };
    $.ajax({
        type: "post",
        url: urlPath,
        data: AddAntiForgeryToken(json_Data),
        success: function (response) {
            if (response != null && response != undefined) {
                $("#RoleList").html(response);

                var TypeID = $("#loader_TypeKey").text();

                var UserType = $('input[name=' + TypeID + ']').filter(':checked').val();

                if (UserType == 2) {
                    $("#FDContent").hide();
                    $("#RoleContent").show();
                    if ($("#OrgContent") != null && $("#OrgContent") != undefined) {
                        $("#OrgContent").hide();
                    }
                }
                else if (UserType == 1) {
                    $("#FDContent").show();
                    $("#RoleContent").hide();
                    if ($("#OrgContent") != null && $("#OrgContent") != undefined) {
                        $("#OrgContent").hide();
                    }
                }
                else if (UserType == 3) {
                    $("#FDContent").hide();
                    $("#RoleContent").hide();
                    if ($("#OrgContent") != null && $("#OrgContent") != undefined) {
                        $("#OrgContent").show();
                    }
                }
                else {
                    $("#FDContent").hide();
                    $("#RoleContent").hide();
                    if ($("#OrgContent") != null && $("#OrgContent") != undefined) {
                        $("#OrgContent").hide();
                    }
                }
            }
            else {
                $("#RoleList").empty();
            }
        }
    });
}

function AddRole() {
    var roleListIDList = $("#roleListIDList").val();
    var roleListPage = $("#roleListPage").val();
    var roleListSort = $("#roleListSort").val();
    var roleListSortDir = $("#roleListSortDir").val();

    if ($("#RoleID").find("option:selected").length > 0) {
        var roleID_Selected = $("#RoleID").find("option:selected").val();
        if (roleListIDList != "" && roleListIDList != undefined) {

            if (roleListIDList.indexOf(roleID_Selected) < 0) {
                roleListIDList = roleListIDList + "|" + roleID_Selected;
            }
        }
        else {
            roleListIDList = roleID_Selected;
        }
        $("#roleListIDList").val(roleListIDList);
    }

    LoadRoleList(roleListIDList, roleListPage, roleListSort, roleListSortDir);
}

function DeleteRole(roleID) {
    var roleListIDList = $("#roleListIDList").val();
    var roleListPage = "1";
    var roleListSort = $("#roleListSort").val();
    var roleListSortDir = $("#roleListSortDir").val();

    var roleListIDList_New = "";
    var roleArray = roleListIDList.split("|");
    roleArray.forEach(function (value, index) {
        if (value != roleID) {
            if (roleListIDList_New == "") {
                roleListIDList_New = value;
            }
            else {
                roleListIDList_New += "|" + value;
            }
        }
    });

    $("#roleListIDList").val(roleListIDList_New);

    LoadRoleList(roleListIDList_New, roleListPage, roleListSort, roleListSortDir);
}
/*------------------Role Functions End---------------------------*/

/*------------------Organization Details Functions Start---------------------------*/
function LoadOrgDetailsList(IDStrList, pageIndex, sort, sortDir) {
    var orgDetailsListIDList = "";
    var orgDetailsListPage = "1";
    var orgDetailsListSort = "";
    var orgDetailsListSortDir = "";

    if (IDStrList != "" && IDStrList != undefined) {
        orgDetailsListIDList = IDStrList;
    }

    if (pageIndex != "" && pageIndex != undefined) {
        orgDetailsListPage = pageIndex;
    }

    if (sort != "" && sort != undefined) {
        orgDetailsListSort = sort;
    }

    if (sortDir != "" && sortDir != c) {
        orgDetailsListSortDir = sortDir;
    }

    var urlPath = $('#loader_ODList').data("request-url");

    var json_Data = { orgDetailsListIDList: "" + orgDetailsListIDList + "", orgDetailsListPage: "" + orgDetailsListPage + "", orgDetailsListSort: "" + orgDetailsListSort + "", orgDetailsListSortDir: "" + orgDetailsListSortDir + "" };
    $.ajax({
        type: "post",
        url: urlPath,
        data: AddAntiForgeryToken(json_Data),
        success: function (response) {
            if (response != null && response != undefined) {
                $("#OrgDList").html(response);
            }
            else {
                $("#OrgDList").empty();
            }
        }
    });
}

function AddOrgDetails() {
    var orgDetailsListIDList = $("#orgDetailsListIDList").val();
    var orgDetailsListPage = $("#orgDetailsListPage").val();
    var orgDetailsListSort = $("#orgDetailsListSort").val();
    var orgDetailsListSortDir = $("#orgDetailsListSortDir").val();

    if ($("#OrgDetailsID").find("option:selected").length > 0) {
        var orgDetailsID_Selected = $("#OrgDetailsID").find("option:selected").val();
        if (orgDetailsListIDList != "" && orgDetailsListIDList != undefined) {

            if (orgDetailsListIDList.indexOf(orgDetailsID_Selected) < 0) {
                orgDetailsListIDList = orgDetailsListIDList + "|" + orgDetailsID_Selected;
            }
        }
        else {
            orgDetailsListIDList = orgDetailsID_Selected;
        }
        $("#orgDetailsListIDList").val(orgDetailsListIDList);
    }

    LoadOrgDetailsList(orgDetailsListIDList, orgDetailsListPage, orgDetailsListSort, orgDetailsListSortDir);
}

function DeleteOrgDetails(roleID) {
    var orgDetailsListIDList = $("#orgDetailsListIDList").val();
    var orgDetailsListPage = "1";
    var orgDetailsListSort = $("#orgDetailsListSort").val();
    var orgDetailsListSortDir = $("#orgDetailsListSortDir").val();

    var orgDetailsListIDList_New = "";
    var orgDetailsArray = orgDetailsListIDList.split("|");
    orgDetailsArray.forEach(function (value, index) {
        if (value != roleID) {
            if (orgDetailsListIDList_New == "") {
                orgDetailsListIDList_New = value;
            }
            else {
                orgDetailsListIDList_New += "|" + value;
            }
        }
    });

    $("#orgDetailsListIDList").val(orgDetailsListIDList_New);

    LoadOrgDetailsList(orgDetailsListIDList_New, orgDetailsListPage, orgDetailsListSort, orgDetailsListSortDir);
}
/*------------------Organization Details Functions End---------------------------*/

/*------------------Specific Functions Start---------------------------*/
function LoadFunctionType(F_ID) {
    $("#FunTypesGroup").hide();
    $("#FunTypesList").empty();
    if (F_ID != "" && F_ID != undefined) {
        var urlPath = $('#loader_FTList').data("request-url");

        var json_Data = { FunID: "" + F_ID + "" };
        $.ajax({
            type: "post",
            url: urlPath,
            data: AddAntiForgeryToken(json_Data),
            success: function (response) {
                if (response != null && response != undefined) {
                    if (response.FTID != null || response.FTID != undefined) {
                        $("#FunTypesList").empty();
                        $("#FunTypesGroup").show();
                        CurrentFunDetailInfo = response;
                        for (var i = 0; i < response.FTName.length; ++i) {
                            var innerHtml = new Array();

                            innerHtml.push("<tr>");
                            innerHtml.push("<td align=\"center\" style=\"text-align:center; width:60px;\">");
                            innerHtml.push("<input type=\"checkbox\" id=\"FunctionTypeCB_" + (i + 1) + "\" onclick=\"chickItemCheckBox(this);\" />");
                            innerHtml.push("</td>");
                            innerHtml.push("<td align=\"left\">" + response.FTName[i] + "</td>");
                            innerHtml.push("</tr>");
                            $("#FunTypesList").append(innerHtml.join(""));
                        }
                    }
                }
                else {
                    $("#FunTypesGroup").hide();
                }
            }
        });
    }
    $("input[type='checkbox'][id=FTCheckAll]").prop('checked', false);
}

function AddFunDetail() {
    var FunDetailInfoList_Str = $("#funDListJson").val();
    var funDListPage = $("#funDListPage").val();
    var funDListSort = $("#funDListSort").val();
    var funDListSortDir = $("#funDListSortDir").val();

    var FunDetailInfoList = null;
    if (FunDetailInfoList_Str != "" && FunDetailInfoList_Str != undefined) {
        FunDetailInfoList = $.parseJSON(FunDetailInfoList_Str);
    }

    if (FunDetailInfoList == null || FunDetailInfoList == undefined) {
        if (CurrentFunDetailInfo != null && CurrentFunDetailInfo != undefined) {
            FunDetailInfoList = new Array();
            FunDetailInfoList.push(CurrentFunDetailInfo);
        }
    }
    else {
        if (CurrentFunDetailInfo != null && CurrentFunDetailInfo != undefined) {
            var isExists = false;
            for (var i = 0; i < FunDetailInfoList.length; ++i) {
                if (FunDetailInfoList[i].FID == CurrentFunDetailInfo.FID) {
                    isExists = true;
                }
            }
            if (!isExists)
                FunDetailInfoList.push(CurrentFunDetailInfo);
        }
    }
    var str_funList = JSON.stringify(FunDetailInfoList);

    LoadFunDList(str_funList, funDListPage, funDListSort, funDListSortDir);

    $("#FunID").val("");
    CurrentFunDetailInfo = null;
    LoadFunctionType("");
    $("input[type='checkbox'][id=FTCheckAll]").prop('checked', false);

    $("#funDListJson").val(str_funList);
}

function SaveFunDetail() {
    var FunDetailInfoList_Str = $("#funDListJson").val();
    var funDListPage = $("#funDListPage").val();
    var funDListSort = $("#funDListSort").val();
    var funDListSortDir = $("#funDListSortDir").val();

    if (FunDetailInfoList_Str != "" && FunDetailInfoList_Str != undefined) {
        var FunDetailInfoList = $.parseJSON(FunDetailInfoList_Str);
        if (CurrentFunDetailInfo != null && CurrentFunDetailInfo != undefined) {
            var isExists = false;
            for (var i = 0; i < FunDetailInfoList.length; ++i) {
                if (FunDetailInfoList[i].FID == CurrentFunDetailInfo.FID) {
                    FunDetailInfoList[i] = CurrentFunDetailInfo;
                }
            }
        }
        var str_funList = JSON.stringify(FunDetailInfoList);
        LoadFunDList(str_funList, funDListPage, funDListSort, funDListSortDir);

        $("#FunID").val("");
        CurrentFunDetailInfo = null;
        LoadFunctionType("");
        $("input[type='checkbox'][id=FTCheckAll]").prop('checked', false);

        $("#FunID").prop("disabled", false);
        $("#FunBtn_Edit").hide();
        $("#FunBtn_Add").show();

        $("#funDListJson").val(str_funList);
    }
}

function DelFunDetail(F_ID) {
    var FunDetailInfoList_Str = $("#funDListJson").val();
    var funDListPage = $("#funDListPage").val();
    var funDListSort = $("#funDListSort").val();
    var funDListSortDir = $("#funDListSortDir").val();

    if (FunDetailInfoList_Str != "" && FunDetailInfoList_Str != undefined) {
        var FunDetailInfoList = $.parseJSON(FunDetailInfoList_Str);
        var temp_FunDetailInfoList = new Array();
        for (var i = 0; i < FunDetailInfoList.length; ++i) {
            if (FunDetailInfoList[i].FID != F_ID) {
                temp_FunDetailInfoList.push(FunDetailInfoList[i]);
            }
        }
        FunDetailInfoList = temp_FunDetailInfoList;

        var str_funList = JSON.stringify(FunDetailInfoList);

        LoadFunDList(str_funList, funDListPage, funDListSort, funDListSortDir);

        $("#funDListJson").val(str_funList);
    }
}

function EditFunDetail(F_ID) {
    var FunDetailInfoList_Str = $("#funDListJson").val();
    $("#FunTypesGroup").hide();
    $("#FunTypesList").empty();

    if (FunDetailInfoList_Str != "" && FunDetailInfoList_Str != undefined) {
        var FunDetailInfoList = $.parseJSON(FunDetailInfoList_Str);

        for (var i = 0; i < FunDetailInfoList.length; ++i) {
            if (FunDetailInfoList[i].FID == F_ID) {
                CurrentFunDetailInfo = FunDetailInfoList[i]
            }
        }
        $("#FunTypesList").empty();
        $("#FunTypesGroup").show();
        for (var i = 0; i < CurrentFunDetailInfo.FTName.length; ++i) {
            var innerHtml = new Array();

            innerHtml.push("<tr>");
            innerHtml.push("<td align=\"center\" style=\"text-align:center; width:60px;\">");
            innerHtml.push("<input type=\"checkbox\" id=\"FunctionTypeCB_" + (i + 1) + "\" onclick=\"chickItemCheckBox(this);\" " + (CurrentFunDetailInfo.FDSelected[i] ? "checked" : "") + " />");
            innerHtml.push("</td>");
            innerHtml.push("<td align=\"left\">" + CurrentFunDetailInfo.FTName[i] + "</td>");
            innerHtml.push("</tr>");
            $("#FunTypesList").append(innerHtml.join(""));
        }
        $("#FunID").val(CurrentFunDetailInfo.FID);
        $("#FunID").prop("disabled", true);
        $("#FunBtn_Edit").show();
        $("#FunBtn_Add").hide();

        var str_funList = JSON.stringify(FunDetailInfoList);

        $("#funDListJson").val(str_funList);
    }
}

function LoadFunDList(jsonStr, pageIndex, sort, sortDir) {
    var jsonStrList = "";
    var funDListPage = "1";
    var funDListSort = "";
    var funDListSortDir = "";

    if (jsonStr != "" && jsonStr != undefined) {
        jsonStrList = jsonStr;
    }

    if (pageIndex != "" && pageIndex != undefined) {
        funDListPage = pageIndex;
    }

    if (sort != "" && sort != undefined) {
        funDListSort = sort;
    }

    if (sortDir != "" && sortDir != c) {
        funDListSortDir = sortDir;
    }

    var urlPath = $('#loader_FDList').data("request-url");

    var json_Data = { FunDetailInfoListStr: jsonStrList, funDListPage: "" + funDListPage + "", funDListSort: "" + funDListSort + "", funDListSortDir: "" + funDListSortDir + "" };
    $.ajax({
        type: "post",
        url: urlPath,
        data: AddAntiForgeryToken(json_Data),
        success: function (response) {
            if (response != null && response != undefined) {
                $("#FunList").html(response);
            }
            else {
                $("#FunList").empty();
            }
        }
    });
}

function chickItemCheckBox(obj) {
    var fdInfo = CurrentFunDetailInfo;
    var indexOfType = $(obj).attr("id").replace("FunctionTypeCB_", "");
    if (!$(obj).is(':checked')) {
        $("input[type='checkbox'][id=FTCheckAll]").prop('checked', false);

        fdInfo.FDSelected[indexOfType - 1] = false;
    }
    else {
        fdInfo.FDSelected[indexOfType - 1] = true;
    }
    CurrentFunDetailInfo = fdInfo;
}
/*------------------Specific Functions End---------------------------*/