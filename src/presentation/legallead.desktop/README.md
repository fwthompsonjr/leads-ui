# legallead.permission.api
The web service delivering business logic to lead sourcing application.  
API behaviors are grouped into six key areas

## Account management functions
 - Allow users to sign-on to application
 - Allow creation of new users
 - Allow user to change password
 - Allow user to unlock account

## Account profile functions
 - Setup mailing and business addresses
 - Manage user preferences

## Account permission functions
 - User opt-in / opt-out to discount programs
 - Support user billing tiers
 
## Payment processing
 - Collect payments
 - Generate invoices
 - Manage subscriptions

## Presentation
 - Shared templates and layouts
 - Common lookup lists
 
## Record Search
 - Route user search request to search providers
 - Monitor status of active searches
 - Deliver search results to customers

## Background processes   
 - health : self-monitoring health check allows constant monitoring of key api functions
 - pricing : data driven manager to keep up with price details
 - records : monitors status of in-flight searches and can auto-correct failed results
 - subscriptions : monitors and responds to common events in the subscription process