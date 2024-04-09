# legallead.desktop   
Desktop version of lead sourcing application   

# Versions  
## vX.Y.Z - ReleaseDate
 - Complete user logout behavior :
     - User can logout of application from account page
     - User can logout of application from context menu

## v3.2.6 ( 2024-04-07 19:09:34 )
 - Update application to support :
     - Session record limits
        - Display friendly message when monthly limit is exceeded   
        - Display friendly message when annual limit is exceeded   
        - Clear alert when limits are within range   

## v3.2.4 ( 2024-03-26 15:50:56 )
 - Update application to support :
     - Session management ( timeout )   
        - Verify session state prior to making api calls   
        - Verify session state prior to performing menu navigation   
        - Display re-authentication modal when session is expiring   
        - Redirect to login page when session has expired   
     - Session management ( lock, unlock )   
        - Verify account lock status during login    
        - Setup 3 failed attempts prior to lock   
        - Setup 15 minute auto-unlock   

## v3.2.3 ( 2024-03-24 20:03:00 )
 - Initial application release delivering behaviors as listed below:
     - Account management functions
     - Account profile functions
     - Account permission functions
     - Payment processing
     - Presentation
     - Record Search

# Behaviors 

## Account management functions
 - Allow users to sign-on to application
 - Allow creation of new users
 - Allow user to change password
 - Allow user to unlock account
 - Protect user session identity

## Account profile functions
 - Setup user name and business name
 - Setup mailing and business addresses
 - Setup account phone numbers
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