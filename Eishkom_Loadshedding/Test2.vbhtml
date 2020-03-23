@code
  'Dim LoadSheddingStages = "1,2,3,4"
  Dim LoadSheddingStages = Split("1,2,3,4", ",")

  Dim db = WebMatrix.Data.Database.Open("Eskom")

  Dim sql As String = ""

  sql = "SELECT ProvinceID, Description FROM Eskom_Provinces WITH (NOLOCK)"

  Dim Provinces = db.Query(sql)

  For Each province In Provinces

    Dim Municipalities = EskomSePush.GetMunicipalities(province(0))

    For Each Muni In Municipalities.Objects
      Response.Write(Muni.Value & " | " & Muni.Text & "<br>")
      Dim count = 0
      sql = "SELECT Count(ID) FROM Eskom_Municipalities WITH (NOLOCK) WHERE ProvinceID = " & province(0) & " AND MunicipalityID = " & Muni.Value
      count = db.QueryValue(sql)
      If count = 0 Then
        sql = "INSERT INTO Eskom_Municipalities ( ProvinceID, MunicipalityID, Description ) VALUES ( " & province(0) & " , " & Muni.Value & " , '" & Muni.Text & "' )"
        db.Execute(sql)
      End If

      Dim suburbsData = EskomSePush.GetSurburbData("", Muni.Value)

      For Each subb In suburbsData.Objects
        Dim count2 = 0
        sql = "SELECT Count(ID) FROM Eskom_Suburbs WITH (NOLOCK) WHERE MunicipalityID = " & Muni.Value & " AND SuburbID = " & subb.id
        count2 = db.QueryValue(sql)
        If count2 = 0 Then


          Try
            sql = "INSERT INTO Eskom_Suburbs ( SuburbID, MunicipalityID, Description ) VALUES ( " & subb.id & " , " & Muni.Value & " , '" & Replace(subb.text, "'", "''") & "' )"
            db.Execute(sql)
          Catch ex As Exception
            Response.Write(sql)
            Response.End()
          End Try

        End If
        If subb.Tot > 0 Then
          For Each stage In LoadSheddingStages
            Response.Write(province(0) & ":" & province(1) & " | Stage #" & stage & "<br>")

            Dim d = EskomSePush.GetSchedules(subb.id, stage - 1)
            If d.Success = True Then
              If d.Objects.Count > 0 Then
                For Each dt In d.Objects
                  @Html.Raw("<h3>Day of month: " & CDate(dt.Day).Day & "</h3>")
                  For Each shed In dt.Schedule

                    Dim count3 = 0
                    sql = "SELECT Count(ID) FROM Eskom_Schedules WITH (NOLOCK) WHERE DayOfMonth = " & CDate(dt.Day).Day & " AND SuburbID = " & subb.id & " AND Stage = " & stage - 1 & " AND ISNULL(StartTime, '') = " & IIf(shed.StartTime = "", "''", "'" & shed.StartTime & "'") & " AND ISNULL(EndTime, '') = " & IIf(shed.EndTime = "", "''", "'" & shed.EndTime & "'")
                    count3 = db.QueryValue(sql)
                    If count3 = 0 Then
                      sql = "INSERT INTO Eskom_Schedules ( SuburbID, DayOfMonth, Stage, StartTime, EndTime ) VALUES ( " & subb.id & " , " & CDate(dt.Day).Day & " , " & stage - 1 & ", " & IIf(shed.StartTime = "", "NULL", "'" & shed.StartTime & "'") & ", " & IIf(shed.EndTime = "", "NULL", "'" & shed.EndTime & "'") & " )"
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
          Next
        End If
      Next

    Next

    Response.Write("<hr>")

    Response.Write("<hr>")
  Next
End Code
