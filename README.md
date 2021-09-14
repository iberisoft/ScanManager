# Scan Manager

This application manages other film digitizing applications via RS232/485.

## Serial Protocol

### Read Commands

Command | Result     | Description
--------|------------|-------------
RD?     | RD=*value* | Get `1` if the scanner is ready to work, otherwise `0`
SC?     | SC=*value* | Get `1` if the scanner is scanning, otherwise `0`
EJ?     | EJ=*value* | Get `1` if the scanner is ejecting, otherwise `0`
RS?     | RS=*value* | Get the resolution value in dpi (VIDAR) or microns (Array)

### Write Commands

Command    | Result | Description
-----------|--------|-------------
SC=*value* | OK     | Set `1` to scan a film, `0` to abort scanning
EJ=*value* | OK     | Set `1` to eject a film
RS=*value* | OK     | Set the resolution value in dpi (VIDAR) or microns (Array)
