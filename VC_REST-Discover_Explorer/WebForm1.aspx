<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="VC_REST_Discover_Explorer.WebForm1"%>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verizon M2M <-> Verizon Cloud API Demo</title>
    <link rel="stylesheet" href="./styles/base.css" type="text/css"/>
    <link rel="stylesheet" href="./styles/jquery-ui-1.10.1.custom.css" type="text/css"/>
    <link rel="stylesheet" href="./styles/jquery-custom.css" type="text/css"/>
    <link rel="stylesheet" href="./styles/compute.css" type="text/css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div class="wrap">
            <div id="top-nav">
                <div class="wrapper">
                    <h1 class="nav-logo">
                        Verizon
                    </h1>
                    <nav class="nav-container">
                        <div class="global-nav">
                            <div class="global-nav-item">
                                <a href="http://cloud.verizon.com">

                                    Verizon Cloud

                                </a>
                            </div>
                            <div class="global-nav-item">
                                <a href="http://cloud.verizon.com/documentation/VerizonCloudComputeAPIReference.htm">

                                    API Documentation

                                </a>
                            </div>
                            <div class="global-nav-item">
                                <a href="http://apiapp.verizon-cloud.net/share/VC_REST-Discover_Explorer.zip">

                                    Download Source of this C# app

                                </a>
                            </div>
                        </div>
                    </nav>
                    <div class="clear"></div>
                </div>
            </div>
        </div>
        <div>
                <table style="width:100%">
                    <tr>
                        <td>
                            <asp:Label ID="lblaccesskey" Text="Access Key" runat="server"> </asp:Label><br />
                            <asp:TextBox ID="txt_accesskey" runat="server" Width="300px" Text=""></asp:TextBox></td>
                        <td>
                            <asp:Label ID="lblsecretkey" Text="Secret Key" runat="server"> </asp:Label><br />
                            <asp:TextBox ID="txt_secretkey" runat="server" Width="300px" Text=""></asp:TextBox></td>
                        <td><asp:Button ID="btn_securedbcreds" Text="    Get from SecureDB    " runat="server" OnClick="btn_securedbcreds_Click" /></td>

                    </tr>
                    <tr>
                        <td style="text-align:right;vertical-align:bottom;">
                            (<b>Optional</b> if more than one cloud space)</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:Label ID="lbl_sdberr" runat="server" Text="" Visible="false"></asp:Label></td>

                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblacctid" Text="x-tmrk-acct" runat="server"> </asp:Label><br />
                            <asp:TextBox ID="txt_acctid" runat="server" Width="300px" Text=""></asp:TextBox></td>
                        <td>
                            <asp:Label ID="lblcloudspace" Text="x-tmrk-cloudspace" runat="server"> </asp:Label><br />
                            <asp:TextBox ID="txt_cloudspace" runat="server" Width="300px" Text=""></asp:TextBox></td>
                        <td>
                            &nbsp;
                        </td>

                    </tr>
                    <tr><td colspan="2">
                        <asp:RadioButtonList ID="rdo_dcuri" runat="server" CellPadding="3" CellSpacing="3" RepeatDirection="Horizontal" Width="700px">
                            <asp:ListItem Text="Culpepper" Value="https://iadg2.cloud.verizon.com" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Miami" Value="https://egwa1.cloud.verizon.com"></asp:ListItem>
                            <asp:ListItem Text="Denver" Value="https://sjca1.cloud.verizon.com"></asp:ListItem>
                            <asp:ListItem Text="Amsterdam" Value="https://amsa1.cloud.verizon.com"></asp:ListItem>
                            <asp:ListItem Text="Private" Value="PrivateURI"></asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:Panel ID="pnl_privatetxt" Visible="false" runat="server">
                            <br />Private Cloud Endpoint URI:&nbsp;<asp:TextBox ID="txt_PrivateEndPoint" runat="server" Width="600px"></asp:TextBox>
                        </asp:Panel>
                        </td>
                        <td>
                            <asp:Button Text="    Check Key Auth    " runat="server" ID="btn_testconn" OnClick="btn_testconn_Click" /><br /><br />
                            <asp:Label ID="lbl_connmsg" runat="server" Visible="false" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr><td colspan="3">
                        <asp:Panel ID="pnl_sts" runat="server" Visible="false">
                            String to Sign: <asp:TextBox ID="txt_stringtosign" runat="server" Text="" ReadOnly="True" Width="1000px"></asp:TextBox>
                        </asp:Panel>
                    </td></tr>
                    
                    <tr>
                        <td colspan="3">                           
                            <div id="div_explorer_options" runat="server" visible="false">
                                <asp:TextBox ID="txt_baseuri" runat="server" Enabled="false" Width="180px"></asp:TextBox>
                                <asp:TextBox ID="txt_exploreuri" runat="server" Width="400px" Text="/api"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                                Content Type: <asp:TextBox ID="txt_contenttype" runat="server" Width="400" Text="application/vnd.terremark.ecloud.root.v1+json"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btn_explore_get" runat="server" Text=" Get " OnClick="btn_explore_get_Click" />&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btn_explore_options" runat="server" Text=" Options " OnClick="btn_explore_options_Click"/>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <div id="div_machines" runat="server" visible="false">

                            </div>
                            <div id="div_explorer" runat="server" visible="false">
                                <asp:Label ID="lbl_explorer" runat="server" Text=""></asp:Label>
                                <asp:TextBox TextMode="MultiLine" ID="txt_multiline" runat="server" Width="900px" Height="500px"></asp:TextBox>
                            </div>
                        </td>
                    </tr>
                </table>
        </div>
    </form>
</body>
</html>