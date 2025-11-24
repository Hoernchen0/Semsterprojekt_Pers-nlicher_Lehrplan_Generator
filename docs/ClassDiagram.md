```mermaid
classDiagram
    %% UI Layer
    class MainWindowViewModel {
        - AiService aiService
        - LernplanService lernplanService
        - AnalyseService analyseService
        - UserSettingsService userSettingsService
        + string UserPrompt
        + LernplanViewModel CurrentLernplan
        + SendPromptAsync()
        + LoadUserDataAsync()
    }

    class LernplanViewModel {
        + ObservableCollection~LernplanEintragViewModel~ Eintraege
    }

    %% Domain Layer
    class Lernplan {
        + Guid Id
        + string Title
        + List~LernplanEintrag~ Eintraege
        + string XmlRaw
    }

    class LernplanEintrag {
        + int Stunde
        + string Thema
        + int DauerMinuten
    }

    class UserData {
        + Guid UserId
        + string Name
        + int GesamtGelerntStunden
    }

    %% Services Layer
    class LernplanService {
        - SqliteRepository repository
        + Task~Lernplan~ ParseXmlAsync(string xml)
        + Task SaveLernplanAsync(Lernplan lernplan)
        + Task~Lernplan~ LoadLatestAsync()
    }

    class AnalyseService {
        - SqliteRepository repository
        + int BerechneGesamtLernzeit(Guid userId)
        + Dictionary~string,int~ ThemaStatistik(Guid userId)
    }

    class AiService {
        + Task~string~ SendePromptAsync(string prompt)
        + Task~string~ HoleXmlAusChatGptAsync(string prompt)
    }

    class UserSettingsService {
        - SqliteRepository repository
        + Task~UserData~ LoadUserAsync()
        + Task SaveUserAsync(UserData data)
    }

    class GoogleCalendarService {
        + Task AuthenticateAsync()
        + Task ExportLernplanToGoogleAsync(Lernplan lernplan)
        + Task~IEnumerable~ LoadCalendarEventsAsync()
    }

    class Backend_Domain {
        + getData(Guid UseriD,Data)
    }

    %% Data Layer
    class SqliteRepository {
        <<interface>>
        + SaveUserAsync(UserData user)
        + LoadUserAsync(Guid id)
        + SaveLernplanAsync(Lernplan lernplan)
        + LoadLernplanAsync(Guid id)
        + LoadAllLernplaeneAsync()
    }

    class USER {
        + UserId PK
        + TEXT Name
        + INT GesamtGelerntStunden
    }

    class LERNPLAN {
        + TEXT LernplanId PK
        + TEXT UserId FK
        + TEXT Title
        + TEXT XmlRaw
        + TEXT CreatedAt
    }

    class LERNPLAN_EINTRAG {
        + TEXT EintragId PK
        + TEXT LernplanId FK
        + INT Stunde
        + TEXT Thema
        + INT DauerMinuten
    }

    class GOOGLE_CALENDAR_MAPPING {
        + TEXT MappingId PK
        + TEXT LernplanEintragId FK
        + TEXT GoogleEventId
    }

    %% Relationships
    MainWindowViewModel --> AiService
    MainWindowViewModel --> LernplanService
    MainWindowViewModel --> AnalyseService
    MainWindowViewModel --> UserSettingsService

    LernplanService --> Backend_Domain 
    AnalyseService --> Backend_Domain 
    UserSettingsService --> Backend_Domain 
    SqliteRepository --> Backend_Domain 
    Backend_Domain --> SqliteRepository
    AiService --> Backend_Domain
    GoogleCalendarService --> Backend_Domain
    SqliteRepository --> Database
    UserData --> Database
    LernplanEintrag --> Database
    Lernplan --> Database

    LernplanViewModel --> MainWindowViewModel

    USER --o LERNPLAN : führt
    LERNPLAN --o LERNPLAN_EINTRAG : besitzt
    LERNPLAN_EINTRAG --o GOOGLE_CALENDAR_MAPPING : verknüpft_mit
    GOOGLE_CALENDAR_MAPPING --o GoogleCalendarService
