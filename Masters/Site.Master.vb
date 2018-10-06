Imports Telerik.Web.UI

Partial Public Class Site
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        CreateMenu()
        'Gets the current page and path
        Dim currentPath As String = Request.ServerVariables.Item("PATH_INFO")
        getBreadCrumbs(currentPath)
    End Sub

    'Creates the menu based on Session("SurveyID") default=0
    Protected Sub CreateMenu()
        Dim SurveyID As String = 0
        If Session("SurveyID") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("SurveyID").ToString) Then
            SurveyID = Session("SurveyID").ToString
        End If
        XmlDataSource1.XPath = "Menu/MenuItems[@SurveyID=" & SurveyID & "]/Item"
        RadMenu1.DataBind()
    End Sub
    'finds the menu item in the radmenu corresponding to the page we are on,
    'invokes the HighlightPath method, then runs the sub that creates the bread crumbs
    Protected Sub getBreadCrumbs(ByVal currentPath As String)

        If currentPath Is Nothing Then
            currentPath = "/default.aspx"
        End If

        If Not String.IsNullOrEmpty(currentPath) Then
            Dim currentItem As RadMenuItem = RadMenu1.FindItemByUrl(currentPath)
            If currentItem IsNot Nothing Then
                'Select the current item and his parents
                currentItem.HighlightPath()
                DataBindBreadCrumbRepeater(currentItem)
            End If
        End If
    End Sub


    'takes in the current menu item from the menu control, creates a list of
    'RadmenuItems, adds the current menu item to the list, and then tries to find
    'the owner or parent of the menu item, then adds that item to the list at index of 0
    ' until we reach the top, and there are no more parents / owners.
    Private Sub DataBindBreadCrumbRepeater(ByVal currentItem As RadMenuItem)

        Dim breadCrumbPath As New List(Of RadMenuItem)()
        While currentItem IsNot Nothing
            breadCrumbPath.Insert(0, currentItem)
            currentItem = TryCast(currentItem.Owner, RadMenuItem)
        End While

        'adds a hyperlink control to a placeholder for each menu item found in the list
        Dim rowcount As Integer = breadCrumbPath.Count
        Dim x As Integer = 1
        For Each item As RadMenuItem In breadCrumbPath
            Dim hl As HyperLink = New HyperLink
            hl.NavigateUrl = item.NavigateUrl
            hl.Text = item.Text
            hl.CssClass = "breadCrumbs"
            phBreadCrumbs.Controls.Add(hl)
            'compares the number of the item we are on with the total number of items
            'adds a space, •, and another space as a separator between links if there are more items
            If x < rowcount Then
                Dim spacer As Literal = New Literal
                spacer.Text = " • "
                phBreadCrumbs.Controls.Add(spacer)
            End If
            x += 1
        Next
    End Sub

  
    'Only shows the site administration menu item if user is authenticated and is a SuperAdmin
    'Also, only shows the Logout button if the user is logged in.
    Private Sub RadMenu1_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadMenuEventArgs) Handles RadMenu1.ItemDataBound
        'Checks if the menu item is the Site Administration menu item, then sets visible = false if the user is not an Admin or Super Admin
        If e.Item.Text = "Administration" Then
            If Not Page.User.IsInRole("SuperAdmin") AndAlso Not Page.User.IsInRole("Admin") Then
                e.Item.Visible = False
            Else
                e.Item.Visible = True

            End If
        ElseIf e.Item.Text = "Site Administration" Or e.Item.Text = "ID Lookups" Then
            e.Item.Target = "_blank"

        End If
        'checks if the menu item is Logout - if the user is not logged in, set visible = false, else true
        If e.Item.Text = "Logout" Then
            If Page.User.Identity.IsAuthenticated Then
                e.Item.Visible = True
            Else
                e.Item.Visible = False
            End If
        End If
    End Sub
End Class