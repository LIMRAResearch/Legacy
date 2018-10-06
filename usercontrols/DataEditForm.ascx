<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DataEditForm.ascx.vb"
    Inherits="SalesSurveysApplication.DataEditForm" %>
    <%@ Register src="ErrorMessage.ascx" tagname="ErrorMessage" tagprefix="uc1" %>
<%@ Register src="NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc2" %>
    <h1>
        <asp:Literal ID="litPageTitle" runat="server">Survey Name</asp:Literal></h1>
    <asp:MultiView ID="MultiView1" runat="server">
       <asp:View ID="vwChooseSurveySeries" runat="server">
       <div class="Container LabelText">Please choose a survey series from the list below:</div>
        <div class="Spacer">
                &nbsp;</div>
       <div class="Container LabelText" style="padding-left: 150px; height: 600px;">Choose a Survey Series:&nbsp;&nbsp;<asp:DropDownList ID="ddlSurveySeries" runat="server">
       <asp:ListItem Value="NA">Choose</asp:ListItem>
       </asp:DropDownList>&nbsp;&nbsp; <asp:Button ID="btnGoToEdit" runat="server" text="Edit Data" />
       </div>
      
        </asp:View>
        <asp:View ID="vwEditData" runat="server">
            <div class="SectionTitle LabelText">
                <asp:Literal ID="litSectionTitle" runat="server">BOLI Over 200 Lives</asp:Literal></div>
            <div class="Container LabelText">
                <div class="LabelText TextLeft LeftLabelColumn">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Planned<br />
                    Recurring<br />
                    (In dollars)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    Excess<br />
                    Over<br />
                    Planned<br />
                    (In dollars)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Target<br />
                    Recurring<br />
                    (In dollars)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    Excess<br />
                    Over Target<br />
                    (In dollars)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    &nbsp;<br />
                    Single<br />
                    (In dollars)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    &nbsp;<br />
                    Face Amount<br />
                    (In thousands)</div>
                <div class="DataColumn TextCenter BottomAlign">
                    &nbsp;<br />
                    &nbsp;<br />
                    Number of<br />
                    Policies</div>
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    Universal Life<br />
                    (Includes Equity Indexed)</div>
                <div class="DataColumn TextCenter">
                    &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
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
                    a. Current Assumption</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row1Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    b. Death Benefit Guarantee</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row2Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    c. Cash Accummulation</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row3Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    d. Other(Specify )</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row4Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>1. Total Universal Life</strong></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row5Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>2. Equity Indexed Universal Life</strong></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row6Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    &nbsp;</div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>3. Variable Life</strong></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row7Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    Variable Universal Life</div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
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
                    e. Protection Focused</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row8Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    f. Cash Accumulation</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row9Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    g. Other(Specify)</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col1" runat="server" Width="100px"></asp:TextBox>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col2" runat="server" Width="100px"></asp:TextBox>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col3" runat="server" Width="100px"></asp:TextBox>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col4" runat="server" Width="100px"></asp:TextBox>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col5" runat="server" Width="100px"></asp:TextBox>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col6" runat="server" Width="100px"></asp:TextBox>
                </div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row10Col7" runat="server" Width="100px"></asp:TextBox>
                </div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>4. Total Variable Universal Life</strong></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row11Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                    Whole Life</div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
                </div>
                <div class="DataColumn TextCenter">
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
                    h. Interest Sensitive</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row12Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row12Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter GrayBackground">
                    <%--<asp:TextBox CssClass="TextRight" ID="Row12Col3" runat="server" Width="100px"></asp:TextBox>--%></div>
                <div class="DataColumn TextCenter GrayBackground">
                    <%--<asp:TextBox CssClass="TextRight" ID="Row12Col4" runat="server" Width="100px"></asp:TextBox>--%></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row12Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row12Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row12Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumnIndented">
                    i. Non-Int. Sensitive</div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row13Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row13Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter GrayBackground">
                    <%--<asp:TextBox CssClass="TextRight" ID="Row13Col3" runat="server" Width="100px"></asp:TextBox>--%></div>
                <div class="DataColumn TextCenter GrayBackground">
                    <%--<asp:TextBox CssClass="TextRight" ID="Row13Col4" runat="server" Width="100px"></asp:TextBox>--%></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row13Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row13Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row13Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>5. Total Whole Life</strong></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row14Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row14Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter GrayBackground">
                   <%-- <asp:TextBox CssClass="TextRight" ID="Row14Col3" runat="server" Width="100px"></asp:TextBox>--%></div>
                <div class="DataColumn TextCenter GrayBackground">
                    <%--<asp:TextBox CssClass="TextRight" ID="Row14Col4" runat="server" Width="100px"></asp:TextBox>--%></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row14Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row14Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row14Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Container">
                <div class="LabelText LeftLabelColumn">
                    <strong>6. GRAND TOTAL</strong></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col1" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col2" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col3" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col4" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col5" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col6" runat="server" Width="100px"></asp:TextBox></div>
                <div class="DataColumn TextCenter">
                    <asp:TextBox CssClass="TextRight" ID="Row15Col7" runat="server" Width="100px"></asp:TextBox></div>
            </div>
            <div class="Spacer">
            </div>
            <div class="Container">
                <div class="LabelText TextLeft LeftLabelColumn">
                &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                &nbsp;
                </div>
                <div class="DataColumn TextCenter">
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
                <div class="DataColumn TextCenter">
                &nbsp;
                </div>
                <div class="DataColumn TextCenter">
                &nbsp;
                </div>
            </div>       
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
    </asp:MultiView>
