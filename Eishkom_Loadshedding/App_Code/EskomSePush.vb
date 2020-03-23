Imports System.IO
Imports System.Net
Imports System.Web.Helpers
Imports HtmlAgilityPack

Public Class EskomSePush
  Private Const BASE_URL As String = "http://loadshedding.eskom.co.za/LoadShedding/"

  Public Shared Function GetStatusText(ByVal status As Integer) As String
    Select Case status
      Case -1
        GetStatusText = "Unknown"
      Case 0
        GetStatusText = "No Loadshedding"
      Case 1
        GetStatusText = "Stage 1 Loadshedding"
      Case 2
        GetStatusText = "Stage 2 Loadshedding"
      Case 3
        GetStatusText = "Stage 3 Loadshedding"
      Case 4
        GetStatusText = "Stage 4 Loadshedding"
      Case 5
        GetStatusText = "Stage 5 Loadshedding"
      Case 6
        GetStatusText = "Stage 6 Loadshedding"
      Case 7
        GetStatusText = "Stage 7 Loadshedding"
      Case 8
        GetStatusText = "Stage 8 Loadshedding"
      Case Else
        GetStatusText = "Unknown"
    End Select
    Return GetStatusText
  End Function

  Public Shared Function GetStatus() As Integer
    Try
      GetStatus = SendRequest("GetStatus", Nothing, "application/json", WebRequestMethods.Http.Get)
    Catch ex As Exception
      GetStatus = -1
    End Try
    Return GetStatus
  End Function

  Public Shared Function GetMunicipalities(ByVal ProvinceReq As Integer) As Municipality_Reply

    Dim objResponseRecord As New Municipality_Reply
    Dim tempList As New List(Of Municipalities)
    Dim result As String = ""

    Try
      result = SendRequest("GetMunicipalities/?Id=" & ProvinceReq, Nothing, "application/json", WebRequestMethods.Http.Get)

      objResponseRecord.Success = True
      objResponseRecord.ErrorMessage = ""
      objResponseRecord.Objects = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Municipalities))(result)
    Catch ex As Exception
      objResponseRecord.Success = False
      objResponseRecord.ErrorMessage = ex.Message
      objResponseRecord.Objects = tempList
    End Try

    Return objResponseRecord
  End Function

  Public Shared Function GetSurburbData(ByVal searchTerm As String, ByVal Municipality As String) As Surburb_Reply

    Dim objResponseRecord As New Surburb_Reply
    Dim tempList As New List(Of GetSurburbData)
    Dim result As String = ""

    Try
      result = SendRequest("GetSurburbData/?pageSize=100000&pageNum=1&searchTerm=" & searchTerm & "&id=" & Municipality, Nothing, "application/json", WebRequestMethods.Http.Get)

      Dim d = Newtonsoft.Json.JsonConvert.DeserializeObject(Of GetSurburbRaw)(result)

      tempList = d.Results

      objResponseRecord.Success = True
      objResponseRecord.ErrorMessage = ""
      objResponseRecord.Objects = tempList
    Catch ex As Exception
      objResponseRecord.Success = False
      objResponseRecord.ErrorMessage = ex.Message
      objResponseRecord.Objects = tempList
    End Try

    Return objResponseRecord
  End Function

  Public Shared Function GetSingleDaySchedules(ByVal Suburb As String, ByVal Stage As String) As Schedule_List

    Dim xx As New Schedule_List

    Dim ss As New List(Of Loadshedding_Schedule)

    Dim result As String = ""

    Try
      'result = SendRequest("GetScheduleM/" & Suburb & "/" & (Stage - 1) & "/_/12", Nothing, "application/json", WebRequestMethods.Http.Get)
      Dim shedUri = BASE_URL & "GetScheduleM/" & Suburb & "/" & Stage & "/_/99999"

      Dim doc As HtmlDocument = New HtmlWeb().Load(shedUri)

      Dim divs = doc.DocumentNode.SelectNodes("//div[@class='scheduleDay']")

      Dim newID = 0

      If divs.Count > 0 Then
        For Each el As HtmlNode In divs
          newID = newID + 1

          If newID > 1 Then
            Exit For
          End If
          Dim dayMonth As HtmlNode = el.SelectSingleNode(".//div[@class='dayMonth']")


          Dim a As New Loadshedding_Schedule

          Try
            Dim newID2 = 0
            For Each el2 As HtmlNode In el.SelectNodes(".//a")


              Dim ScheduleTime = Split(el2.InnerText, " - ")

              Dim b As New Auto_Arm_Schedule_Hour
              b.Sched_No = newID2
              newID2 = newID2 + 1

              b.StartTime = ScheduleTime(0)
              b.EndTime = ScheduleTime(1)


              a.Schedule.Add(b)

            Next
          Catch ex As Exception
            Dim b As New Auto_Arm_Schedule_Hour
            b.Sched_No = -1
            b.StartTime = ""
            b.EndTime = ""

            a.Schedule.Add(b)
          End Try



          a.ID = newID
          a.Day = Trim(Replace(Replace(Replace(Replace(dayMonth.InnerText, vbLf, ""), vbCr, ""), vbNewLine, ""), vbCrLf, ""))

          ss.Add(a)
        Next

        xx.Success = True

        xx.Schedule = Stage
        xx.SuburbID = Suburb

        xx.ErrorMessage = ""
        xx.Objects = ss
      Else
        xx.Success = False

        xx.Schedule = Stage
        xx.SuburbID = Suburb

        xx.ErrorMessage = "No schedules found."
        xx.Objects = ss
      End If
    Catch ex As Exception
      xx.Success = False

      xx.Schedule = Stage
      xx.SuburbID = Suburb

      xx.ErrorMessage = ex.Message
      xx.Objects = ss
    End Try

    Return xx
  End Function

  Public Shared Function GetSchedules(ByVal Suburb As String, ByVal Stage As String) As Schedule_List

    Dim xx As New Schedule_List

    Dim ss As New List(Of Loadshedding_Schedule)

    Dim result As String = ""

    Try
      'result = SendRequest("GetScheduleM/" & Suburb & "/" & (Stage - 1) & "/_/12", Nothing, "application/json", WebRequestMethods.Http.Get)
      Dim shedUri = BASE_URL & "GetScheduleM/" & Suburb & "/" & Stage & "/_/99999"

      Dim doc As HtmlDocument = New HtmlWeb().Load(shedUri)

      Dim divs = doc.DocumentNode.SelectNodes("//div[@class='scheduleDay']")

      Dim newID = 0

      If divs.Count > 0 Then
        For Each el As HtmlNode In divs
          newID = newID + 1
          Dim dayMonth As HtmlNode = el.SelectSingleNode(".//div[@class='dayMonth']")


          Dim a As New Loadshedding_Schedule

          Try
            Dim newID2 = 0
            For Each el2 As HtmlNode In el.SelectNodes(".//a")


              Dim ScheduleTime = Split(el2.InnerText, " - ")

              Dim b As New Auto_Arm_Schedule_Hour
              b.Sched_No = newID2
              newID2 = newID2 + 1

              b.StartTime = ScheduleTime(0)
              b.EndTime = ScheduleTime(1)


              a.Schedule.Add(b)

            Next
          Catch ex As Exception
            Dim b As New Auto_Arm_Schedule_Hour
            b.Sched_No = -1
            b.StartTime = ""
            b.EndTime = ""

            a.Schedule.Add(b)
          End Try



          a.ID = newID
          a.Day = Trim(Replace(Replace(Replace(Replace(dayMonth.InnerText, vbLf, ""), vbCr, ""), vbNewLine, ""), vbCrLf, ""))

          ss.Add(a)
        Next

        xx.Success = True

        xx.Schedule = Stage
        xx.SuburbID = Suburb

        xx.ErrorMessage = ""
        xx.Objects = ss
      Else
        xx.Success = False

        xx.Schedule = Stage
        xx.SuburbID = Suburb

        xx.ErrorMessage = "No schedules found."
        xx.Objects = ss
      End If
    Catch ex As Exception
      xx.Success = False

      xx.Schedule = Stage
      xx.SuburbID = Suburb

      xx.ErrorMessage = ex.Message
      xx.Objects = ss
    End Try

    Return xx
  End Function

  Public Shared Function SendRequest(uri As String, jsonDataBytes As Byte(), contentType As String, method As String) As String
    Dim fullUri = New Uri(BASE_URL & uri)

    Dim req As HttpWebRequest = WebRequest.Create(fullUri)
    req.Timeout = 9000000
    req.ContentType = contentType
    req.Method = method
    req.Accept = contentType

    If method = "POST" Then
      Dim stream = req.GetRequestStream()
      stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
      stream.Close()
    End If

    Dim response = req.GetResponse.GetResponseStream

    Dim reader As New StreamReader(response)
    Dim res = reader.ReadToEnd()
    reader.Close()
    response.Close()

    Return res
  End Function
End Class

Public Enum Provinces
  EASTERN_CAPE = 1
  FREE_STATE = 2
  GAUTENG = 3
  KWAZULU_NATAL = 4
  LIMPOPO = 5
  MPUMALANGA = 6
  NORTH_WEST = 7
  NORTHERN_CAPE = 8
  WESTERN_CAPE = 9
End Enum

Public Class Municipality_Reply
  Public Property Success As Boolean
  Public Property ErrorMessage As String
  Public Property Objects As List(Of Municipalities)
End Class

Public Class Municipalities
  'Public Property Selected As String 'we don't use this, so remove
  Public Property Text As String
  Public Property Value As String
End Class

Public Class Surburb_Reply
  Public Property Success As Boolean
  Public Property ErrorMessage As String
  Public Property Objects As List(Of GetSurburbData)
End Class

Public Class GetSurburb
  Public Property Total As Integer
  Public Property Objects As List(Of GetSurburbData)
End Class

Public Class GetSurburbData
  Public Property id As Integer
  Public Property text As String
  Public Property Tot As Integer
End Class

Public Class GetSurburbRaw
  Public Property Total As Integer
  Public Property Results As List(Of GetSurburbData)
End Class

'Schedule stuffs
Public Class Schedule_List
  Public Property Success As Boolean
  Public Property Schedule As Integer
  Public Property SuburbID As Integer
  Public Property ErrorMessage As String
  Public Property Objects As List(Of Loadshedding_Schedule)
End Class

Public Class Loadshedding_Schedule
  Public Property ID As Integer
  Public Property Day As String
  Public Property Schedule As List(Of Auto_Arm_Schedule_Hour)
  Public Sub New()
    Schedule = New List(Of Auto_Arm_Schedule_Hour)
  End Sub
End Class

Public Class Auto_Arm_Schedule_Hour
  Public Property Sched_No As Int32
  Public Property StartTime As String
  Public Property EndTime As String
End Class