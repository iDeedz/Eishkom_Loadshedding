# Eishkom_Loadshedding
Eskom Loadshedding

-------------------------

Project to get current Loadshedding Status, Municipalities, Surburbs and Shedules from Eskom Uri.

-------------------------

Calls

http://loadshedding.eskom.co.za/LoadShedding/GetStatus
Returns Integer

http://loadshedding.eskom.co.za/LoadShedding/GetMunicipalities/?Id={ProvinceID}
Returns Json

http://loadshedding.eskom.co.za/LoadShedding/GetSurburbData/?pageSize=100000&pageNum=1&searchTerm={SearchValue}&id={MunicipalityID}
Returns Json

http://loadshedding.eskom.co.za/LoadShedding/GetScheduleM/{SuburbID}/{Stage}/_/99999
Returns Html
