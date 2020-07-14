<%@ Page Title="" Language="C#" MasterPageFile="~/Master/LeftBar.master" AutoEventWireup="true" CodeBehind="Admin_Code.aspx.cs" Inherits="SSWARE.Form.Admin.Code" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">        
        window.onload = function () {

            topbarOnload();
            leftbarOnload();

            var cols = [];

            cols.push(new column("id", "id", "120", false));
            cols.push(new column("Name", "NameTag", "120", true));
            cols.push(new column("Memo", "Memotag", "360", true, true));
            
            var ly = new layout(cols);
            var dataurl = "Admin_Code.aspx?type=data";
            var uploadurl = "Admin_Code.aspx?type=upload";
            setGrid("testgrid", ly, dataurl, uploadurl);

            var dbl = function (row, col) {
                console.log("in dbl : " + row.index + ", " + col.name);
            };
            
            grids["testgrid"].setOnDblClick(dbl);
                        
            var dataurl2 = "Admin_Code.aspx?type=tdata";
            setTree("testtree", dataurl2);
            trees["testtree"].setOnClick(function (obj) {
                console.log(obj.Name);
                console.log("getValue : " + trees["testtree"].getValue(10).Name);
            });

            trees["testtree"].setIcon("User", "/Tree/Images/Person.png");
            trees["testtree"].setIcon("Group", "/Tree/Images/Department.png");

            setEditor("testeditor", "<span>a<span>b<span>ddd</span><span>e</span></span><span>cc<span>f</span><span>g</span></span></span>");
            
        };

        var ar = function () {
            var row = {};
            row.Name = "Test";
            row.Memo = "Test Memo";
            grids["testgrid"].addRow(row);
        };

        var save = function () {
            grids["testgrid"].save();
        };

        var saveEditor = function () {
            //console.log(editors["testeditor"].save());
            editors["testeditor"].save()
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CphLeftBar" runat="server">

    <div id="Contents" class="Contents">
        <input type="button" value="addrow" onclick="javascript:ar();" />
        <input type="button" value="save" onclick="javascript:save();" />
        <div id="testgrid" style="width:500px; height:500px;">TestGrid</div>        
        <div id="testtree" style="width:500px; height:500px;">TestTree</div>
        <input type="button" value="saveEditor" onclick="javascript:saveEditor();" />
        <div id="testeditor" style="width:1000px; height:500px;">TestEditor</div>
    </div>
    
</asp:Content>