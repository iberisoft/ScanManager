# Scan Manager

This application manages other film digitizing applications via Modbus.

## Modbus Protocol

### Read Discrete Inputs

Address | Description
--------|-------------
0       | `true` if the scanner is ready to work

### Read Coils

Address | Description
--------|-------------
0       | `true` if the scanner is scanning
1       | `true` if the scanner is ejecting


### Write Coils

Address | Description
--------|-------------
0       | Write `true` to scan a film, `false` to abort scanning
1       | Write `true` to eject a film
