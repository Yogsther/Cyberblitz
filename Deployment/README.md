## Deployment

This is the automatic deployment system for the
server and website. This system automatically listens to
webhooks for the `stable` and `nightly` branches. When an update
is pushed, it pulls, updates the server `/ServerBuild` and restart. It also zips the game build 
`/Build`, names it correctly `(CyberBlitz_Stable_V1.2.zip)` and makes it avalible to
download from the website. 

The servers are run with Docker so we can easily restart/update and monitor them. 

This system is built using NodeJS

### Endpoints
`cyberblitz.okdev.se` Website

`stable.cyberblitz.okdev.se` Stable Live server

`nightly.cyberblitz.okdev.se` Latest Live server

`cyberblitz.ygstr.com` Prototype server


### Internal ports

* `810 nightly`
* `811 stable`
* `812 website`