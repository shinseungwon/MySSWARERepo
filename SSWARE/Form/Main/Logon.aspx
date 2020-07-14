<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="SSWARE.Form.Main.Logon" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript">
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Logon Page</h3>
            <table>
                <tr>
                    <td>E-mail address:</td>
                    <td>
                        <asp:TextBox ID="UserEmail" runat="server" /></td>
                    <td>
<%--                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                            ControlToValidate="UserEmail"
                            Display="Dynamic"
                            ErrorMessage="Cannot be empty."
                            runat="server" />--%>
                    </td>
                </tr>
                <tr>
                    <td>Password:</td>
                    <td>
                        <asp:TextBox ID="UserPass" TextMode="Password"
                            runat="server" />
                    </td>
                    <td>
<%--                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                            ControlToValidate="UserPass"
                            ErrorMessage="Cannot be empty."
                            runat="server" />--%>
                    </td>
                </tr>
                <tr>
                    <td>Remember me?</td>
                    <td>
                        <asp:CheckBox ID="Persist" runat="server" /></td>
                </tr>
            </table>
            <asp:Button ID="Submit1" OnClick="Logon_Click" Text="Log On"
                runat="server" />
            <p>
                <asp:Label ID="Msg" ForeColor="red" runat="server" />
            </p>
        </div>
    </form>
</body>
</html>
