<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master"
    CodeBehind="Corrections.aspx.vb" Inherits="SalesSurveysApplication.Corrections" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../../../usercontrols/NotAuthorized.ascx" TagName="NotAuthorized"
    TagPrefix="uc1" %>
<%@ Register Src="../../../usercontrols/ErrorMessage.ascx" TagName="ErrorMessage"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                ShowSummary="False" />
            <div class="Container">
            </div>
            <div class="Container LabelText">
                Please choose a survey by selecting all of the parameters below:</div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container LabelText RedText">
                <asp:Literal ID="litStatus" runat="server"></asp:Literal></div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Date Series Required"
                        ControlToValidate="ddlDateSeries" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Date Series:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlDateSeries" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Workbook Required"
                        ControlToValidate="ddlWorkbooks" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Workbook:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlWorkbooks" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Distribution Channel Required"
                        ControlToValidate="ddlDistributions" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Page / Worksheet:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlDistributions" runat="server" AppendDataBoundItems="True"
                        Width="200px">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtMOR"
                        Display="Dynamic" ErrorMessage="Integers Only for MOR" Operator="DataTypeCheck"
                        Type="Integer">*</asp:CompareValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="MOR Required"
                        ControlToValidate="txtMOR" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;MOR:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtMOR" runat="server" Width="50px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnContinue" runat="server" Text="Edit" Width="87px" />
                </div>
            </div>
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <div class="Spacer">
                &nbsp;</div>
            <div class="Spacer">
                <strong>Contributions ($000) by Product</strong></div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container LabelText">
                <div class="LabelText TextLeft LeftLabelColumn">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    403(b)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    <br />
                    457</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Other NFP</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Total NFP</div>
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>Variable individual Annuities:</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Variable Subaccounts</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row1Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row1Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row1Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row1Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Fixed Subaccounts</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row2Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row2Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row2Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row2Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Total Variable Annuities</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row3Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row3Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row3Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row3Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>Fixed Individual Annuities</strong>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row4Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row4Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row4Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row4Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>Group Annuity Contracts(exclude GIA-only contracts):</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    GIA</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row5Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row5Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row5Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row5Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    SIA</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row6Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row6Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row6Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row6Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Total</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row7Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row7Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row7Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row7Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>Group Annuity GIA-only Contracts</strong>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row8Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row8Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row8Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row8Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>Mutual Funds</strong>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row9Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row9Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row9Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row9Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>Total Contributions</strong>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row10Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row10Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row10Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row10Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>Assets($000)</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Full Service</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row11Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row11Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row11Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row11Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Administrative Recordkeeping Only</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row12Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row12Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row12Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row12Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Investment Only</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row13Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row13Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row13Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row13Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>Total Assets($000)</strong>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row14Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row14Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row14Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row14Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>Participants</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    New for the quarter</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row15Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row15Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row15Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row15Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Total Participants</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row16Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row16Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row16Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row16Col4" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div style="clear: both;"></div>
           <div class="Spacer">
                &nbsp;</div>
                <div class="Container" ><hr style="width: 1100px;" /></div>
            <div class="Spacer">
                <strong>Market Segments</strong><br />&nbsp;&nbsp;Please note that all figures in this section should correspond to Full Service Business only.</div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container LabelText">
                <div class="LabelText TextLeft LeftLabelColumn">
                    &nbsp;
                </div>
               
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Contributions<br />
                    ($000)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    <br />
                    Assets<br />
                    ($000)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Total<br />
                    Participants</div>
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>403(b)</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    K - 12</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row17Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row17Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row17Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    College and University</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row18Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row18Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row18Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Hospital and Healthcare</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row19Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row19Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row19Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Other 403(b)</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row20Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row20Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row20Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>457</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    State and Local Government</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row21Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row21Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row21Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    K - 12</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row22Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row22Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row22Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    College and University</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row23Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row23Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row23Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Hospital and Healthcare</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row24Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row24Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row24COl3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Other 457</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row25Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row25Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row25Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>Other NFP</strong></div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    401(a) Government</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row26Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row26Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row26Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    401(a) Hospital & Healthcare</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row27Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row27Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row27Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    401(a) Other NFP</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row28Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row28Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row28Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    401(k) Hospital & Healthcare</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row29Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row29Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row29Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    401(k) Other NFP</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row30Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row30Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row30Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Other Hospital & Healthcare</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row31Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row31Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row31Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    Other NFP</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row32Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row32Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="P1Row32Col3" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
                   <div class="Spacer">
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                    <asp:Button ID="btnUpdate" runat="Server" Text="Save Changes" />
                </div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                </div>
                
              </div>
        </asp:View>
        <asp:View ID="vwEdit2" runat="server">
            <div class="Spacer">
                &nbsp;</div>

            <div class="Spacer">
                <strong>Market Segments</strong></div>

            <div class="Spacer">
                &nbsp;</div>
            <div class="Container LabelText">
                <div class="LabelText TextLeft LeftLabelColumn">
                    <strong>Sub-Total Assets by MktSgt</strong></div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;
                    Assets ($000)</div>
            </div>
            <div class="Container LabelText">
                <div class="LabelText LeftLabelColumnIndented">
                        K  - 12</div>
                    <div class="DataColumn TextCenter">
                        <asp:TextBox CssClass="TextRight" ID="P2Row1Col1" runat="server" Width="100px"></asp:TextBox></div>    
            </div>
            <div class="Container LabelText">
                <div class="LabelText LeftLabelColumnIndented">
                        College & University</div>
                    <div class="DataColumn TextCenter">
                        <asp:TextBox CssClass="TextRight" ID="P2Row2Col1" runat="server" Width="100px"></asp:TextBox></div>    
            </div>
            <div class="Container LabelText">
                <div class="LabelText LeftLabelColumnIndented">
                        Hospital</div>
                    <div class="DataColumn TextCenter">
                        <asp:TextBox CssClass="TextRight" ID="P2Row3Col1" runat="server" Width="100px"></asp:TextBox></div>    
            </div>
            <div class="Container LabelText">
                <div class="LabelText LeftLabelColumnIndented">
                        Government</div>
                    <div class="DataColumn TextCenter">
                        <asp:TextBox CssClass="TextRight" ID="P2Row4Col1" runat="server" Width="100px"></asp:TextBox></div>    
            </div>
            <div class="Container LabelText">
                <div class="LabelText LeftLabelColumnIndented">
                        Other  NFP</div>
                    <div class="DataColumn TextCenter">
                        <asp:TextBox CssClass="TextRight" ID="P2Row5Col1" runat="server" Width="100px"></asp:TextBox></div>    
            </div>
            <div class="Container">
                <div class="DataColumn TextCenter">
                    <asp:Button ID="btnUpdate2" runat="Server" Text="Save Changes" />
                </div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                    <asp:Button ID="btnCancel2" runat="server" Text="Cancel" />
                </div>
                
              </div>
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc2:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
