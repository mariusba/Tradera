# Tradera #

.Net5 implementation of crypto exchanges listener + notifier


## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Remarks](#remarks)

## Installation 

In order to start this service, simple boot it up and select Run/Debug configuration to start.


### Prerequisites

1. Required:
    * [.NET5 ](https://dotnet.microsoft.com/download/dotnet/5.0) - Hosting boundle runtime for hosting the backend service
2. Optional:   
      * Development IDE
        * [Visual Studio](https://visualstudio.microsoft.com/downloads) 
        * [Visual Studio Code](https://code.visualstudio.com/)  
        * [Rider](https://www.jetbrains.com/rider/)

## Usage 

1.Start Up service
2. POST to https://localhost:5001/jobs/start with exchange/pair identifier to start background process
3. GET https://localhost:5001/prices with query params of identifier to retrieve highest/lowest price in last 100 events
4. POST to https://localhost:5001/jobs/stop with exchange/pair identifier to stop background process
    
## Remarks 


Currently, only binance is supported. Adding a new exchange is straightforward:
- Implement IWebSocketConfigurator interface
- Define exchange response
- Implement IMapper interface
- add new Enum to ExchangeName
- Inject new implementations

Things to improve:
- More logging,
- More tests,
- refactoring ExchangeWrapper to be injectable
- Adding INotificationStrategyService which would be imlementable per strategy/pair
