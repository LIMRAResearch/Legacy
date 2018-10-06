Public Partial Class DateSeriesmnt
    Inherits System.Web.UI.Page

    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not User.IsInRole("SuperAdmin") Then
            MultiView1.SetActiveView(vwNotAuthorized)
        Else
            If Not Page.IsPostBack Then
                LoadSurveys(CType(Session("OnyxID"), Integer))
            End If
        End If

    End Sub

    Private Sub LoadSurveys(ByVal OnyxID As Integer)

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()


        Dim query = From u In db.admUserSurveys Join p In db.dSurveys _
                        On u.SurveyID Equals p.SurveyID _
                        Order By p.SurveyName _
                        Where u.OnyxID = OnyxID _
                        Select p.SurveyID, p.SurveyName

        'Sets the properties for the survey drop down list
        'Adds the first item, maps the value and text properties to items in the query
        'Sets the datasource of the drop down list to the query results, displays the appropriate view
        ddlSurveys.Items.Clear()
        ddlSurveys.AppendDataBoundItems = True
        ddlSurveys.Items.Add(New ListItem("Please Choose", "NA"))
        ddlSurveys.DataTextField = "SurveyName"
        ddlSurveys.DataValueField = "SurveyID"
        ddlSurveys.DataSource = query
        ddlSurveys.DataBind()
        MultiView1.SetActiveView(vwDefault)

    End Sub

    Private Sub LoadDateSeries()


        ddlDateSeries.Items.Clear()
        ddlDateSeries.AppendDataBoundItems = True
        ddlDateSeries.Items.Add(New ListItem("Please Choose", "NA"))
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Dim query = From p In db.dSurveySeries _
                    Where p.SurveyID = ddlSurveys.SelectedValue _
                    Order By p.DateID Descending _
                    Select p.DateID, p.SurveySeriesID

        'loops through the query, truncates the date to get rid of the time value
        If query.Count > 0 Then
            For Each item In query
                ddlDateSeries.Items.Add(New ListItem(item.DateID.ToShortDateString, item.SurveySeriesID))
            Next
        End If

        MultiView1.SetActiveView(vwDateSeriesSelect)

    End Sub

    Protected Sub btnSelectDateSeries_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectDateSeries.Click

        lblSurveyIDSelect.Text = ddlSurveys.SelectedItem.ToString
        LoadDateSeries()

    End Sub

    Protected Sub btnEditDateSeries_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEditDateSeries.Click

        lblSurveyIDEdit.Text = ddlSurveys.SelectedItem.ToString
        RadDtPckEditDateSeries.SelectedDate = ddlDateSeries.SelectedItem.Text
        MultiView1.SetActiveView(vwDateSeriesEdit)


    End Sub

    Protected Sub btnCancelEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEditCancel.Click

        MultiView1.SetActiveView(vwDefault)

    End Sub


    Protected Sub btnSaveEditChanges_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveEditChanges.Click


        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
        Dim SurveyID As Integer = ddlSurveys.SelectedValue

        Dim DateInput As Date
        Dim YearInput As Integer
        Dim QtrInput As Integer

        DateInput = RadDtPckEditDateSeries.SelectedDate
        YearInput = DatePart(DateInterval.Year, DateInput)
        QtrInput = DatePart(DateInterval.Quarter, DateInput)


        Try
            Dim qrySurveySeries = (From ss In db.dSurveySeries _
                                    Select ss Where ss.SurveyID = SurveyID And ss.DateID = DateInput.ToString("yyyy-MM-dd")).Single()

            qrySurveySeries.DateID = DateInput.ToString("yyyy-MM-dd")
            qrySurveySeries.Year = YearInput
            qrySurveySeries.Quarter = QtrInput


            db.SubmitChanges()

            pnlMessageEdit.Visible = True
            lblLastEditSurvey.Text = SurveyID
            lblLastEditDate.Text = DateInput.ToString("yyyy-MM-dd")

            MultiView1.SetActiveView(vwDefault)

        Catch ex As Exception
            displayMessageBox("Date for Survey NOT FOUND")
        End Try


    End Sub

    Private Sub displayMessageBox(ByVal msg As String)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessage", "alert('" + msg + "');", True)

    End Sub


    Protected Sub btnAddDateSeries_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddDateSeries.Click

        lblSurveyIDAdd.Text = ddlSurveys.SelectedItem.ToString
        RadDtPckAddDateSeries.Clear()
        MultiView1.SetActiveView(vwDateSeriesAdd)


    End Sub

    Protected Sub btnCancelAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelAdd.Click

        MultiView1.SetActiveView(vwDefault)

    End Sub

    Protected Sub btnSaveAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveAdd.Click

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
        Dim SurveyID As Integer = ddlSurveys.SelectedValue

        Dim DateInput As Date
        Dim YearInput As Integer
        Dim QtrInput As Integer
        Dim Exists As Boolean
        Dim ValidDate As Boolean
        Dim DayInput As Integer
        Dim MonthInput As Integer


        DateInput = RadDtPckAddDateSeries.SelectedDate
        YearInput = DatePart(DateInterval.Year, DateInput)
        QtrInput = DatePart(DateInterval.Quarter, DateInput)
        DayInput = DatePart(DateInterval.Day, DateInput)
        MonthInput = DatePart(DateInterval.Month, DateInput)

        
        Try

            Dim qrySurveySeries = From ss In db.dSurveySeries _
                                   Select ss Where ss.SurveyID = SurveyID And ss.DateID = DateInput.ToString("yyyy-MM-dd")

            Exists = qrySurveySeries.Count <> 0

            If Exists = False Then

                ValidDate = ValidFrequencyDate(SurveyID, MonthInput, DayInput)

                If ValidDate = True Then

                    Dim dateSeries As New dSurveySeries With {.SurveyID = SurveyID, _
                                                        .DateID = DateInput.ToString("yyyy-MM-dd"), _
                                                        .Year = YearInput, _
                                                        .Quarter = QtrInput}
                    db.dSurveySeries.InsertOnSubmit(dateSeries)
                    db.SubmitChanges()


                    pnlMessageAdd.Visible = True
                    lblLastAddSurvey.Text = SurveyID
                    lblLastAddDate.Text = DateInput.ToString("yyyy-MM-dd")

                    MultiView1.SetActiveView(vwDefault)

                End If
            Else

                displayMessageBox("Date Series already exists for the Survey")

            End If

        Catch ex As Exception
            displayMessageBox("Date for Survey NOT FOUND")
        End Try

    End Sub

    Function ValidFrequencyDate(ByVal SurveyID As Integer, ByVal MonthInput As Integer, ByVal DayInput As Integer)

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
        Dim Valid As Boolean

        Try

            'Get Survey Frequency
            Dim getSurveyFrequency = From ds In db.dSurveys Where ds.SurveyID = SurveyID _
                                     Select ds.SurveyFrequency

            If getSurveyFrequency.count = 1 Then


                Dim getValidFrequencyDate = From f In db.dFrequencies _
                                     Select f Where f.Frequency = getSurveyFrequency.First _
                                     And f.AllowedMonth = MonthInput _
                                     And f.AllowedDay = DayInput

                Valid = getValidFrequencyDate.Count = 1

                If Not Valid Then

                    displayMessageBox("Date is not Valid for Survey Frequence - " & getSurveyFrequency.First)

                End If
            Else

                displayMessageBox("Survey Not Found - " & SurveyID)

            End If


        Catch ex As Exception

            displayMessageBox("Frequency NOT found")

        End Try

        Return Valid

    End Function

End Class