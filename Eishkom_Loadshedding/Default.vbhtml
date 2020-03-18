@imports HtmlAgilityPack
@Code
  Layout = "~/_SiteLayout.vbhtml"
  PageData("Title") = "Home Page"

  Dim ProvinceReq = Request("province")
  If ProvinceReq = "" Then
    ProvinceReq = 0
  End If
  Dim Municipality = Request("municipality")
  Dim SuburbText = Request("SuburbText")
  Dim Suburb = Request("suburb")

  Dim db = Database.Open("Eskom")

  Dim sql As String = ""

  sql = "SELECT ProvinceID, Description FROM Eskom_Provinces WITH (NOLOCK)"

  Dim Provinces = db.Query(sql)
  Dim LoadsheddingStage = -1

  Try
    LoadsheddingStage = SePush.GetStatus
  Catch ex As Exception
    LoadsheddingStage = -1
  End Try
End Code

<h1>Eishkom Loadshedding:</h1>

<form method="post" name="ls">
  <div class="form-group">
    <h3>
      Status: 
      <i class="fa fa-power-off @Html.Raw(IIf(IIf(LoadsheddingStage > -1, LoadsheddingStage - 1, LoadsheddingStage) > 0, "text-danger", "text-success"))" aria-hidden="true"></i>
      @SePush.GetStatusText(IIf(LoadsheddingStage > -1, LoadsheddingStage - 1, LoadsheddingStage))
      <input type="hidden" name="status" value="@LoadsheddingStage" />
    </h3>
  </div>
  @code
    Try
      If LoadsheddingStage > -1 Then
        Dim count = 0
        Dim status = -1
        sql = "SELECT TOP 1 ISNULL(Count(ID), 0), ISNULL(Stage, -1) FROM Eskom_LoadsheddingStatus WITH (NOLOCK) WHERE CONVERT(DATE, Timestamp) = CONVERT(DATE, GETDATE()) GROUP BY Stage"
        Dim LSStatus = db.QuerySingle(sql)
        If Not IsNothing(LSStatus) Then
          count = LSStatus(0)
          status = LSStatus(1)
        Else
          count = 0
          status = -1
        End If

        If count = 0 Then
          Try
            sql = "INSERT INTO Eskom_LoadsheddingStatus ( Timestamp, Stage ) VALUES ( GETDATE() , " & IIf(LoadsheddingStage > -1, LoadsheddingStage - 1, LoadsheddingStage) & " )"
            db.Execute(sql)
          Catch ex As Exception
            Response.Write(sql)
            Response.End()
          End Try
        ElseIf count = 1 And status <> IIf(LoadsheddingStage > -1, LoadsheddingStage - 1, LoadsheddingStage) And IIf(LoadsheddingStage > -1, LoadsheddingStage - 1, LoadsheddingStage) > -1 Then
          sql = "UPDATE Eskom_LoadsheddingStatus SET Stage = " & IIf(LoadsheddingStage > -1, LoadsheddingStage - 1, LoadsheddingStage) & ", Timestamp = GETDATE() WHERE CONVERT(DATE, Timestamp) = CONVERT(DATE, GETDATE()) "
          db.Execute(sql)
        End If
      End If
    Catch ex As Exception
      @ex.Message
    End Try
  End Code

  <div class="form-group">
    <label for="province">Select Province</label>
    <select class="form-control" onchange="ls.submit()" name="province">
      <option value="0" @Html.Raw(IIf(ProvinceReq = 0, "selected", ""))>Select Province...</option>
      @code
        For Each Province In Provinces
      End code
      <option value="@Province(0)" @Html.Raw(IIf(ProvinceReq = Province(0), "selected", ""))>@Province(1)</option>
      @code
      Next
      End Code
    </select>
  </div>

  <div class="form-group">
    <label for="municipality">Municipalities</label>
    @code
      If ProvinceReq <> "" Then
        Dim Municipalities = SePush.GetMunicipalities(ProvinceReq)
        @*@Json.Encode(Municipalities)*@
    End Code
    <select class="form-control" onchange="ls.submit()" name="municipality">
      <option value="0" @Html.Raw(IIf(Municipality = 0, "selected", ""))>Select Municipality...</option>
      @code
        For Each Muni In Municipalities.Objects
          Dim count = 0
          sql = "SELECT Count(ID) FROM Eskom_Municipalities WITH (NOLOCK) WHERE ProvinceID = " & ProvinceReq & " AND MunicipalityID = " & Muni.Value
          count = db.QueryValue(sql)
          If count = 0 Then
            sql = "INSERT INTO Eskom_Municipalities ( ProvinceID, MunicipalityID, Description ) VALUES ( " & ProvinceReq & " , " & Muni.Value & " , '" & Muni.Text & "' )"
            db.Execute(sql)
          End If
      End code
      <option value="@Muni.Value" @Html.Raw(IIf(Municipality = Muni.Value, "selected", ""))>@Muni.Text</option>
      @code
      Next
      End Code
    </select>
    @code
    Else
    End code
    <p>Please select Province</p>
    @code
    End If
    End Code
  </div>
  <div class="form-inline">
    @*<code>@Municipality</code>*@
    @code
      If Municipality <> "" Then
        Dim suburbsData = SePush.GetSurburbData("", Municipality)
    End Code
    <br />
    <select class="form-control" onchange="ls.submit()" name="suburb">
      <option value="0">Select Suburb...</option>
      @code
        For Each subb In suburbsData.Objects
          Dim count = 0
          sql = "SELECT Count(ID) FROM Eskom_Suburbs WITH (NOLOCK) WHERE MunicipalityID = " & Municipality & " AND SuburbID = " & subb.id
          count = db.QueryValue(sql)
          If count = 0 Then


            Try
              sql = "INSERT INTO Eskom_Suburbs ( SuburbID, MunicipalityID, Description ) VALUES ( " & subb.id & " , " & Municipality & " , '" & Replace(subb.text, "'", "''") & "' )"
              db.Execute(sql)
            Catch ex As Exception
              Response.Write(sql)
              Response.End()
            End Try

          End If
          If subb.Tot > 0 Then
      End code
      <option value="@subb.id" @Html.Raw(IIf(Suburb = subb.id, "selected", ""))>@subb.text</option>
      @code
          End If
        Next
      End Code
    </select>
    @code
      Else
    End code
    <p>Please select Municipality</p>
    @code
      End If
    End Code
  </div>
  @*<code>@Suburb</code>*@
  @code
    Dim BASE_URL = "http://loadshedding.eskom.co.za/LoadShedding/"
    If Suburb <> "" And Suburb <> "0" Then
      LoadsheddingStage = 4 'Overwrite Default Loadshedding Stage

      'Dim s As String = BASE_URL & "/GetScheduleM/" & Suburb & "/" & (LoadsheddingStage - 1) & "/_/12"
      '@("http://loadshedding.eskom.co.za/LoadShedding/GetScheduleM/" & Suburb & "/" & (LoadsheddingStage - 1) & "/_/12")
      'Dim d = SePush.SendRequest("GetScheduleM/" & Suburb & "/" & (LoadsheddingStage - 1) & "/_/12", Nothing, "application/json", WebRequestMethods.Http.Get)

      If LoadsheddingStage > 0 Then


        Dim d = SePush.GetSchedules(Suburb, LoadsheddingStage)
        '@Json.Encode(d)
        If d.Success = True Then
          If d.Objects.Count > 0 Then
            For Each dt In d.Objects
            @Html.Raw("<h3>" & CDate(dt.Day).Day & "</h3>")
              For Each shed In dt.Schedule

                Dim count = 0
                sql = "SELECT Count(ID) FROM Eskom_Schedules WITH (NOLOCK) WHERE DayOfMonth = " & CDate(dt.Day).Day & " AND SuburbID = " & Suburb & " AND Stage = " & LoadsheddingStage & " AND ISNULL(StartTime, '') = " & IIf(shed.StartTime = "", "''", "'" & shed.StartTime & "'") & " AND ISNULL(EndTime, '') = " & IIf(shed.EndTime = "", "''", "'" & shed.EndTime & "'")
                count = db.QueryValue(sql)
                If count = 0 Then
                  sql = "INSERT INTO Eskom_Schedules ( SuburbID, DayOfMonth, Stage, StartTime, EndTime ) VALUES ( " & Suburb & " , " & CDate(dt.Day).Day & " , " & LoadsheddingStage & ", " & IIf(shed.StartTime = "", "NULL", "'" & shed.StartTime & "'") & ", " & IIf(shed.EndTime = "", "NULL", "'" & shed.EndTime & "'") & " )"
                  db.Execute(sql)
                End If

                If shed.Sched_No = -1 Then
                @Html.Raw("<p>No Loadshedding</p><br>")
                Else
                @Html.Raw("<p>" & shed.Sched_No & " - Start: " & shed.StartTime & " | End: " & shed.EndTime & "</p><br>")
                End If

              Next
            @Html.Raw("<hr>")
            Next
          End If
        Else
        @Html.Raw(d.ErrorMessage)
        End If
      End If
    End If
  End Code
</form>

