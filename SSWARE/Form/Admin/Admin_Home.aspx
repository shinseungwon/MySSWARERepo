<%@ Page Title="" Language="C#" MasterPageFile="~/Master/LeftBar.master" AutoEventWireup="true" CodeBehind="Admin_Home.aspx.cs" Inherits="SSWARE.Form.Main.HomeAdmin" %>
<%--관리자 페이지 홈--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        
    </style>
    <script type="text/javascript">
        
        window.onload = function () {

        };

        function send() {

            var obj = {
                "Target": "ft_test(123)",
                "param": "999",                
                "o_rtn": "",
                "o_str": ""
            };            
            
            SendAjax(obj, cb);
        };

        function cb(val) {
            alert("callback : " + val);            
            var x = JSON.parse(val);            
            alert(x[0].name);
        };
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CphLeftBar" runat="server">
    
    <div id="Contents" runat="server">
        <asp:Button ID="b1" runat="server" text="b1"/>
        <asp:Button ID="b2" runat="server" text="b2"/>
        <asp:Button ID="b3" runat="server" text="b3"/>
        <asp:Button ID="b4" runat="server" text="b4"/>
        <asp:Button ID="b5" runat="server" text="b5"/>
        <asp:Button ID="b6" runat="server" text="b6"/>        
    </div>
    
</asp:Content>
