@echo off

IF EXIST package.json GOTO startBatch

:download
(
    @echo Downloading honkit...
    call npm i honkit
    cls
    @echo Import plugin...  1/12
    call npm i gitbook-plugin-folding-chapters
    cls
    @echo Import plugin...  2/12
    call npm i gitbook-plugin-theme-code
    cls
    @echo Import plugin...  3/12
    call npm i gitbook-plugin-header-generator-rmp
    cls
    @echo Import plugin...  4/12
    call npm i gitbook-plugin-youtube
    cls
    @echo Import plugin...  5/12
    call npm i gitbook-plugin-toolbar
    cls
    @echo Import plugin...  6/12
    call npm i gitbook-plugin-signature
    cls
    @echo Import plugin...  7/12
    call npm i gitbook-plugin-hints
    cls
    @echo Import plugin...  8/12
    call npm i gitbook-plugin-timeline
    cls
    @echo Import plugin...  9/12
    call npm i gitbook-plugin-glossary-tooltip
    cls
    @echo Import plugin... 10/12
    call npm i gitbook-plugin-betterchinese
    cls
    @echo Import plugin... 11/12
    call npm i gitbook-plugin-code-pro
    cls
    @echo Import plugin... 12/12
    call npm i gitbook-plugin-page-toc-button
    cls

    :BookJson(
        IF EXIST book.json (
            @echo book.json exist, do you want to reset?
            @echo [ Y/N ]
            CHOICE /c YN /n >nul

            IF errorlevel==2 (GOTO startBatch)
        )
        @echo Generate book.json...
        @echo >"book.json

        @echo {> book.json
        @echo   "title": "my title",>> book.json
        @echo   "author": "name",>> book.json
        @echo   "language": "en",>> book.json
        @echo   "plugins": [>> book.json
        @echo     "theme-code",>> book.json
        @echo     "folding-chapters",>> book.json
        @echo     "toolbar",>> book.json
        @echo     "code-pro",>> book.json
        @echo     "hints",>> book.json
        @echo     "gitbook-plugin-betterchinese",>> book.json
        @echo     "page-toc-button">> book.json
        @echo   ],>> book.json
        @echo   "pluginsConfig": {>> book.json
        @echo     "theme-default": {>> book.json
        @echo       "showLevel": false>> book.json
        @echo     },>> book.json
        @echo     "page-toc-button": {>> book.json
        @echo       "maxTocDepth": 2,>> book.json
        @echo       "minTocSize": 2 >> book.json
        @echo     }>> book.json
        @echo   }>> book.json
        @echo }>> book.json
    )
    call npx honkit init

    GOTO startBatch
)

:startBatch
(
    cls
    @echo ------------command--------------
    @echo 'D' - Download and init Honkit
    @echo 'B' - Build html
    @echo 'S' - startBatch serve
    @echo 'C' - End batch process 
    @echo ---------------------------------

    CHOICE /c DBSC
    IF errorlevel == 4 GOTO endCmd
    IF errorlevel == 3 GOTO Serve
    IF errorlevel == 2 GOTO Build
    IF errorlevel == 1 GOTO download

)

:Build
(
    call npx honkit build
    GOTO startBatch
)

:Serve
(
    @echo use Ctrl-C to end the serve
    call npx honkit serve
    GOTO startBatch
)

:endCmd