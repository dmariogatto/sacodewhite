# SA CodeWhite

Mobile app to view South Australian public hospital dashboards.

- [Ambulance Service Dashboard ](https://www.sahealth.sa.gov.au/wps/wcm/connect/public+content/sa+health+internet/about+us/our+performance/our+hospital+dashboards/about+the+ambulance+service+dashboard/ambulance+service+dashboard)
- [Emergency Department Dashboard](https://www.sahealth.sa.gov.au/wps/wcm/connect/public+content/sa+health+internet/about+us/our+performance/our+hospital+dashboards/about+the+ed+dashboard/emergency+department+dashboard)

## Components

### Azure Functions API & Data Scraper

To massage the dashboard data (which is a bunch of JSON files that are bound to Kendo UI components) & to support push notifications there is an Azure Functions API. The API simply setups up a couple CRON jobs to fetch the JSON files from each dashboard, combine them into one object and save the result as a blob. 

### Mobile App

The mobile app uses the combined dashboard data to display the current status of the Emergency Departments in South Australian public hospitals.

|                                                                        |                                                            |
|------------------------------------------------------------------------|------------------------------------------------------------| 
| ![Pulse](/assets/screens/pulse.png)                                    | ![Ambulance Dashboard](/assets/screens/ambo_dashboard.png) |
| ![ED Dashboard](/assets/screens/ed_dashboard.png)                      | ![Settings](/assets/screens/settings.png)                  |
| ![Ambulance Dashboard (Dark)](/assets/screens/ambo_dashboard_dark.png) | ![Ambulance FMC](/assets/screens/ambo_fmc.png)             |
| ![ED FMC](/assets/screens/ed_fmc.png)                                  |                                                            |
