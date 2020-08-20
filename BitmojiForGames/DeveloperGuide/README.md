![image alt text](image_0.png)

Developer Guide

* * *


Version 1.5

Aug 7, 2020

**Bitmoji for Games Developer Guide**

Copyright © 2019 Snap Inc. All rights reserved. 

Snap Inc. confidential information. Do not share.

This document contains Snap Inc. confidential information and intellectual property and is meant for its intended recipient(s) only. No part of this document may be disclosed or reproduced in any form without express permission from Snap Inc. All information subject to change without notice.

[Snap Developer Terms of Service](https://kit.snapchat.com/portal/eula?viewOnly=true)

[Snap Inc. Terms of Service](https://www.bitmoji.com/support/terms.html)

[[TOC]]

# Glossary

<table>
  <tr>
    <td>Term</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>BFG</td>
    <td>Bitmoji for Games.</td>
  </tr>
  <tr>
    <td>Snap Kit</td>
    <td>A developer kit for some of Snapchat’s best features.</td>
  </tr>
  <tr>
    <td>Login Kit</td>
    <td>A toolkit for authorizing access to Snapchat accounts and Bitmoji avatars.</td>
  </tr>
  <tr>
    <td>Bitmoji Kit</td>
    <td>A toolkit to access bitmoji resources for both 2D and 3D Bitmoji avatars.</td>
  </tr>
  <tr>
    <td>GraphQL</td>
    <td>GraphQL is a query language for APIs and a runtime for fulfilling those queries with your existing data.</td>
  </tr>
  <tr>
    <td>ECDSA</td>
    <td>A cryptographic algorithm to ensure data is protected and can only be accessed by its rightful owners.</td>
  </tr>
  <tr>
    <td>JWT</td>
    <td>An Internet standard for creating JSON-based access tokens that assert some number of claims.</td>
  </tr>
</table>


# About this guide

Welcome to Bitmoji for Games!

The *Bitmoji for Games Developer Guide *is an in-depth reference guide for developers to learn about the BFG platform. It includes conceptual and technical descriptions of all components that make up BFG. 

You can learn how to build, test, and publish your games with BFG integration.

# Audience

This document is intended for game developers and partners who’d like to create and launch their games using BFG. 

# Introduction

This BFG platform provides a set of resources for integrating Bitmoji avatars into games. Players can connect their Snapchat account and authorize the game to use their personal avatar.

# Getting started

## Prerequisites

You will need access to the following:

1. [Snapchat account](https://whatis.snapchat.com/) user account with verified email.

2. A linked Bitmoji that has been created and saved to the Snapchat account.

3. [Snap Kit](https://kit.snapchat.com/) developer portal, which will allow you to authenticate with Snapchat to access the Bitmoji avatar.

## Step 1 - Set up your Snap Kit account

1. Go to [Snap Kit](https://kit.snapchat.com/) and [login to the developer portal](https://kit.snapchat.com/portal) with your Snapchat account.

2. Create an [Organization](https://kit.snapchat.com/portal/orgs).

3. Create a new [app](https://kit.snapchat.com/portal/apps) and place it under your organization.

4. Toggle on Login Kit and Bitmoji Kit for your application.

## Step 2 - Integrate Login Kit into your game

1. Follow the Snap Kit instructions to use [Login Kit](https://docs.snapchat.com/docs/login-kit) in your game depending on the platform in which your game is being built.

2. In order to properly authenticate accounts in your game, you will need the clientId from the [app](https://kit.snapchat.com/portal/apps) you created.

3. Once you have the clientId, follow the [Login Kit](https://docs.snapchat.com/docs/login-kit) guide to set up authentication for your game.

## Step 3 - Access Bitmoji avatars

1. Send an email to [games@bitmoji.com](mailto:games@bitmoji.com) with the development client id for non-confidential oauth2 flow for your app. You should receive a response within 24 hours that it has been whitelisted for development.

2. Login Kit will provide an access token that can be passed to the various Bitmoji API endpoints to retrieve the player’s avatarId and display Bitmoji models or stickers.

3. For 3D Bitmoji models, grab the user’s avatar Id from the Snap Kit GraphQL endpoint [https://api.snapkit.com/v1/me](https://api.snapkit.com/v1/me). Use the [https://bitmoji.api.snapchat.com/bitmoji-for-games/model](https://bitmoji.api.snapchat.com/bitmoji-for-games/model/) endpoint to retrieve the avatar by passing the obfuscated id as a parameter. You must have the OAuth 2.0 token returned from Login Kit and pass it in the header.

The level of detail for the model can be passed in as a parameter depending on the quality required. If you wish to use a test avatar not associated with a Snapchat account, use the [https://bitmoji.api.snapchat.com/bitmoji-for-games-staging/test_avatars](https://bitmoji.api.snapchat.com/bitmoji-for-games-staging/test_avatars) endpoint to retrieve a list of UUIDs that can be passed into the previous endpoint for retrieval of the GLB for the model.

4. Users can see their Bitmoji in 2D stickers throughout your application, to enhance their experience. If you’d like to learn more about this stickers service, please contact us at [games@bitmoji.com](mailto:games@bitmoji.com).

## Step 4 - Develop your game

## Step 5 - Launch your game

1. Send an email to [games@bitmoji.com](mailto:games@bitmoji.com) with the production client id for non-confidential oauth2 flow for your app. You should receive a response within 24 hours that it has been whitelisted for production.

2. Once your game is ready for release, use the Snap Kit Developer Portal to submit the app you created for review.

# Authorizing BFG

In order to authorize and use Bitmojis in your game, you will need to implement either Login Kit or Scan to Auth. [Login Kit](https://docs.snapchat.com/docs/login-kit) has documentation for client integrations. Going forward, we’ll explain how to use the Scan to Auth method for access to Bitmojis.

## Scan to Authorize

Scan to Auth requires server to server authentication in order to produce the Snapcodes needed to be able to access 2D and 3D Bitmojis for players. You will need to set up server to server authentication to create your snapcode. 

### Connect with Snap

#### Create an ECDSA Key Pair

1. Create the private key: $ openssl ecparam -name prime256v1 -genkey -noout -out private-key.pem

2. From your private key, create the public key: $ openssl ec -in private-key.pem -pubout -out public-key.pem

#### Send the Public Key to Your Snap Advocate

Your advocate at Snap will assign an iss and a kid to you:

* iss

    * A UUID string created to uniquely identify your organization

* kid

    * The iss-version number

    * version number is to support key rotation in the future

With the previous steps, you will now be able to authenticate with the Snap Kit Snapcode creation endpoint.

### Construct the Content for the Snapcode

In order for a Snapcode to be displayed, a POST request body must be constructed and sent to the Snap Kit endpoint.

<table>
  <tr>
    <td>POST Body</td>
  </tr>
  <tr>
    <td>postBody = {
   "clientId": snapkit_client_id,
   "state": generated_oauth2_state,
   "redirectUrl": oauth2_redirect_url,
   "codeChallenge": generated_oauth2_code_challege,
   "scopes": oauth2_scopes
}</td>
  </tr>
</table>


Each of the five parameters in the POST body are required. Refer to[ https://www.oauth.com/oauth2-servers/server-side-apps/authorization-code/](https://www.oauth.com/oauth2-servers/server-side-apps/authorization-code/) for the OAuth2.0 flows and 

[https://www.oauth.com/oauth2-servers/pkce/authorization-request/](https://www.oauth.com/oauth2-servers/pkce/authorization-request/) for the code challenge and code verifier.

#### Parameters

clientId

* String - Client ID from the developer portal ([https://sdk.snapkit.com](https://sdk.snapkit.com/))

state

* String - Generated oauth2 state Ex: A UUID

redirectUrl

* String - A redirect URL to your server to receive the authorization code when a player grants permission in the OAuth2.0 flow

codeChallenge

* String - A generated String for the OAuth2.0 code challenge (You will need to generate a code verifier first for this session to use later as well)

scopes

* An array of scope urls that let your application declare which Login Kit features it wants access to

    * [https://auth.snapchat.com/oauth2/api/user.display_name](https://auth.snapchat.com/oauth2/api/user.display_name): Grants access to the player’s Snapchat display name

    * [https://auth.snapchat.com/oauth2/api/user.bitmoji.avatar](https://auth.snapchat.com/oauth2/api/user.bitmoji.avatar): Grants access to the player’s Bitmoji avatar; toggleable by the player

* For Bitmoji access in your game, you will need the avatar scope, but if you want to display the Snapchat username, you will also need the display name scope.

<table>
  <tr>
    <td>Scope</td>
  </tr>
  <tr>
    <td>scopes = [
   "https://auth.snapchat.com/oauth2/api/user.display_name",
   "https://auth.snapchat.com/oauth2/api/user.bitmoji.avatar"
]</td>
  </tr>
</table>


### Construct the Authentication Header

The authentication header must have the following format: Bearer <Unsigned Token>.<Signature>

The token is a[ JWT token](https://jwt.io/introduction/). The following are the field descriptions which you can use to generate the signature.

<table>
  <tr>
    <td>Authentication Header Body</td>
  </tr>
  <tr>
    <td>header = '{
   "alg": "ES256",
   "typ": "JWT",
   "kid": <string: <3pa organization id>-v<key version>> // <key version> is 1, 2, etc., to support key rotation
}'

payload = '{
   "iss": <string: 3PA organization ID>,
   "aud": "API:SnapKit:Scan",
   "exp": <int32: expiry time in seconds from 1970-01-01T00:00:00Z UTC>,
   "iat": <int32: issued time in seconds from 1970-01-01T00:00:00Z UTC>,
   "hash": <string: sha256Hex of request postBody in all lower case hex chars>
}'</td>
  </tr>
</table>


In order to sign the token, the following code is required:

<table>
  <tr>
    <td>privateKey = <string: private key>
unsignedToken = base64.rawurlencode(header) + '.' + base64.rawurlencode(payload)
signature = base64.rawurlencode(ecdsaSigner.sign(privateKey, unsignedToken))
authorizationHeader = 'Bearer ' + unsignedToken + '.' + signature
request.header(‘X-Snap-Kit-S2S-Auth’, authorizationHeader)</td>
  </tr>
</table>


The following snippets are examples of a header, payload, and generated token:

#### Header

<table>
  <tr>
    <td>{
   "alg": "ES256",
   "kid": "dcb57664-94ba-469e-ab4c-e2468ad218b9-v1",
   "typ": "JWT"
}</td>
  </tr>
</table>


#### Payload

<table>
  <tr>
    <td>{
   "aud": "API:SnapKit:Scan",
   "exp": 1529640526,
   "hash": "85f07ad28767eaab637a5f78ed3ebc23f58595d08db14df7d8f2df312106cab9",
   "iat": 1527630526,
   "iss": "dcb57664-94ba-469e-ab4c-e2468ad218b9"
}</td>
  </tr>
</table>


#### Generated Token

<table>
  <tr>
    <td>Bearer eyJhbGciOiJFUzI1NiIsImtpZCI6ImRjYjU3NjY0LTk0YmEtNDY5ZS1hYjRjLWUyNDY4YWQyMThiOS12MSIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJQdWJsaWNTdG9yeUtpdEFQSSIsImV4cCI6MTUyOTY0MDUyNiwiaGFzaCI6Ijg1ZjA3YWQyODc2N2VhYWI2MzdhNWY3OGVkM2ViYzIzZjU4NTk1ZDA4ZGIxNGRmN2Q4ZjJkZjMxMjEwNmNhYjkiLCJpYXQiOjE1Mjc2MzA1MjYsImlzcyI6ImRjYjU3NjY0LTk0YmEtNDY5ZS1hYjRjLWUyNDY4YWQyMThiOSJ9.OUKz6vPYH6VQVk0KK30qOaUWhvc50WH1HAa-VoXxHnkuG5JmZRFPizbDdEOjK8qSDNGPKuo_X--4MM7faBgLGw</td>
  </tr>
</table>


JWT offers many[ token signing libraries](https://jwt.io/) in various languages. To simplify token signing, choose the one that fits easily with your tech stack.

### Send a Request to the Snap Kit Snapcode Creation Endpoint

Finally, your server should be able to make a request against Snap Kit’s API to request the generation of a Snapcode for user authentication.

<table>
  <tr>
    <td>Endpoint: https://api.snapkit.com/v1/scan/auth
Method: POST
Header: X-Snap-Kit-S2S-Auth : Bearer generated_token_in_previous_step
//This is the POST body we constructed in the previous steps
body:
{
   "clientId": snapkit_client_id,
   "state": generated_oauth2_state,
   "redirectUrl": oauth2_redirect_url,
   "codeChallenge": generated_oauth2_code_challege,
   "Scopes": oauth2_scopes
}

Result: a snapcode in PNG image bytes</td>
  </tr>
</table>


### Display the Snapcode to the Player

After you create and retrieve the Snapcode, you will need to display this snapcode to the player and instruct them to scan the Snapcode to process the OAuth2.0 flow and be able to authorize their Bitmoji for use in your game.

Displaying the code and informing players how to use it should be in accordance with our brand guidelines.

### Retrieve the Access Token and Refresh Token

Refer to[ https://kit.snapchat.com/docs/tutorials/login-kit/web/](https://kit.snapchat.com/docs/tutorials/login-kit/web/) for more information.

The OAuth2.0[ authorization grant flow](https://tools.ietf.org/html/rfc6749#section-1.3.1) we follow for this integration provides* refreshable offline access tokens*. These access tokens enable players to access features of the Login Kit SDK and require the player to only authorize your application once.

#### Get the Auth Code from the RedirectURL

Once the player scans the Snapcode you provided and grants the permissions to your game, your server will receive the authorization code in the OAuth2.0 redirectURL you provided previously.

https://redirectUrl?code=authortization_code&state=state_provided

**redirectUrl: **The URL you provided when you created the Snapcode and whitelisted with the Snap Kit application.

**state: **The OAuth2.0 state you provided when creating the Snapcode.

**code**: The authorization code you need to retrieve the access token and refresh token.

#### Get Access Token and Refresh Token

With the authorization code and **code verifier** (generated in the previous steps and used to generate the code challenge in order to create the Snapcode), you will be able to make a request to Snap Kit to retrieve an access token and refresh token for the player. Store the tokens in your server database and refresh the access token when needed.

The authorization code expires after 10 minutes. The required OAuth2.0 parameters for this step are outlined below.

<table>
  <tr>
    <td>Parameter</td>
    <td>Definition</td>
  </tr>
  <tr>
    <td>client_id</td>
    <td>The clientId Snap assigned to your application when you signed up for Snap Kit. The value is a 36 character alphanumeric string.</td>
  </tr>
  <tr>
    <td>client_secret</td>
    <td>The client secret Snap assigned to your application when you signed up for Snap Kit. The value is a BASE64 URL encoded string.</td>
  </tr>
  <tr>
    <td>redirect_uri</td>
    <td>The redirect URI that you requested for your application.</td>
  </tr>
  <tr>
    <td>grant_type</td>
    <td>Possible values include "authorization_code" or “refresh_token”.</td>
  </tr>
  <tr>
    <td>code</td>
    <td>The authorization code received from the authorization server.</td>
  </tr>
  <tr>
    <td>code_verifier</td>
    <td>Generated in the previous steps and used to generate the code challenge in order to create the Snapcode.</td>
  </tr>
</table>


#### Requesting an Access Token for A User

Build a POST request with the header and payload (which includes the authorization code from the OAuth2.0 response) and the access token can be retrieved.

<table>
  <tr>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>POST Request</td>
    <td>https://accounts.snapchat.com/accounts/oauth2/token</td>
  </tr>
  <tr>
    <td>Header</td>
    <td>Authorization: Basic BASE16(<client_id>:<client_secret>)</td>
  </tr>
  <tr>
    <td>Payload</td>
    <td>grant_type=authorization_code&redirect_uri=<redirect_uri>&code=<code></td>
  </tr>
</table>


A successful response will have the following format.

<table>
  <tr>
    <td>// Success Response Body
{
   access_token: <string>,
   refresh_token: <string>,
   expires_in: <time in seconds>
}</td>
  </tr>
</table>


If an error occurs, an error response will be generated and returned.

<table>
  <tr>
    <td>// Error Response Body
{
   error: <ascii error code>,
   error_description: <human readable error description>
}</td>
  </tr>
</table>


Now the access token and refresh token should be available to your server. You can use them to access username, 2D, and 3D Bitmoji avatars by making requests to the appropriate BFG API endpoints and passing your clientId and the player’s access token.

# Authentication

Authentication to BFG services is completed through OAuth 2.0 inside the Snap Kit Developer Portal. Once you create a new application, you will be provided with a clientId for both the development and production environments. When using Login Kit to authenticate, the clientId is passed along with the user credentials and an OAuth 2.0 access token and refresh token are returned to be used to grant access to the various BFG APIs that will returned 3D models or 2D stickers.

To properly authenticate players in your game, please follow the correct [Login Kit](https://docs.snapchat.com/docs/login-kit) instructions for the platform you are developing on (iOS, Android, Web) or follow the Scan to Authorize instructions if using that service.

# Permissions

In order to properly develop with Bitmoji, you will need to gain the permissioning scope for your application to use Bitmoji assets. To do this, when viewing your application in the Snap Kit developer portal you must enable Login Kit and Bitmoji Kit in your application settings. This will give your game access to be able to get both 3D Bitmoji models as well as 2D stickers.

# Developer guidelines

This section describes guidelines that all developers need to adhere to when developing and launching their games using BFG. 

## Content and design

Refer to the Bitmoji for Games Usage Guidelines to ensure your game adheres to the nature of the brand.

## User privacy

* Your game must comply with all applicable data protection and privacy laws, statutes, ordinances, rules, and regulations of any jurisdiction throughout the world, including the EU General Data Protection Regulation.

* Your game should not collect any personal data about a user except to the extent necessary to play the game and to comply with Applicable Law. Your game may not collect any sensitive data of any kind.

* You must provide and maintain at all times a functioning link to a privacy policy that complies with applicable law in the Snap Games Publishing Tool (if developing with the Snap Games platform).

* You will not display users’ display names, identifier, or usernames to players outside the originating conversation group without written permission from Snapchat.

## Testing and QA

### Mobile Games

We recommend testing your game on both iOS and Android thoroughly across all supported OS versions. This can be challenging on Android so we recommend testing across a set of devices representing the full spectrum of Android GPUs. We also recommend testing under unreliable, low bandwidth, or flakey network conditions.

# API Reference

## Request Avatar Id from Snap Kit

Method: POST

Endpoint: [https://api.snapkit.com/v1/me](https://api.snapkit.com/v1/me)

GraphQL Query: {me{bitmoji{id}}}

Headers

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>Authorization</td>
    <td>Yes</td>
    <td>String</td>
    <td>Generated OAuth2.0 token from the successful authorization response from Login Kit.</td>
  </tr>
</table>


### Request Avatar Id

The following example shows how to fetch an avatar id using the POST method:

<table>
  <tr>
    <td>Request</td>
  </tr>
  <tr>
    <td>curl -X POST https://api.snapkit.com/v1/me \
--data '{"query": "{me{bitmoji{id}}}"}' \
-H "Content-Type: application/json" \
-H 'Authorization: Bearer {your_access_token}'</td>
  </tr>
</table>


Here’s the response if the service finds the id:

<table>
  <tr>
    <td>Response</td>
  </tr>
  <tr>
    <td>{
   "data": {
      "me": {
         "bitmoji": {
            "id": "obfuscated_avatar_id"
         }
      }
   },
   "errors": []
}</td>
  </tr>
</table>


If there’s an error:

<table>
  <tr>
    <td>401 Unauthorized - Denied</td>
  </tr>
  <tr>
    <td>{
  "message": "invalid_token"
}</td>
  </tr>
</table>


This is an error from Snap-kit, showing the access token is not valid. Please check the bearer token under the authorization header.

<table>
  <tr>
    <td>404 Not Found</td>
  </tr>
</table>


Please check the user has a Bitmoji account.

<table>
  <tr>
    <td>500 Internal Server Error</td>
  </tr>
</table>


There is an error from the service. Please double check the request is valid and try again.

## Request Avatar Model from BFG

Method: GET

Endpoint: [https://bitmoji.api.snapchat.com/bitmoji-for-games/model](https://bitmoji.api.snapchat.com/bitmoji-for-games/model/)

Headers

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>Authorization</td>
    <td>Yes</td>
    <td>String</td>
    <td>Generated OAuth2.0 token from the successful authorization response from Login Kit.</td>
  </tr>
</table>


### Request Avatar

The following example shows how to fetch an avatar using the GET method:

<table>
  <tr>
    <td>Request</td>
  </tr>
  <tr>
    <td>curl -X GET 'https://bitmoji.api.snapchat.com/bitmoji-for-games/model?avatar_id={obfuscated_avatar_id}&lod=3' \
-H 'Accept-Encoding: gzip, deflate' \
-H 'Authorization: Bearer {your_access_token}' </td>
  </tr>
</table>


Here’s the response if the service finds a model:

<table>
  <tr>
    <td>Response</td>
  </tr>
  <tr>
    <td>This endpoint will return the 3d avatar model for the authenticated user or the test avatar.</td>
  </tr>
</table>


If there’s an error:

<table>
  <tr>
    <td>401 Unauthorized</td>
  </tr>
</table>


Ensure that your clientId for the application has been whitelisted by the Bitmoji team.

<table>
  <tr>
    <td>401 Unauthorized - Denied</td>
  </tr>
  <tr>
    <td>{
  "message": "invalid_token"
}</td>
  </tr>
</table>


This is an error from Snap Kit, showing the access token is not valid. Please check the bearer token under the authorization header.

<table>
  <tr>
    <td>404 Not Found</td>
  </tr>
</table>


Please check the user has a Bitmoji account.

<table>
  <tr>
    <td>500 Internal Server Error</td>
  </tr>
</table>


There is an error from the service. Please double check the request is valid and try again.

#### Parameters

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>lod</td>
    <td>No</td>
    <td>number</td>
    <td>The level of detail for a model. We currently have LOD 0 for the highest level of detail and LOD 3 for the level of detail seen in Snap Games.</td>
  </tr>
  <tr>
    <td>avatar_id</td>
    <td>Yes</td>
    <td>string</td>
    <td>The UUID of one of the sample avatars provided in the list of dummy IDs or the player’s obfuscated avatarId.</td>
  </tr>
  <tr>
    <td>scope</td>
    <td>No</td>
    <td>string</td>
    <td>The scope of the model to retrieve. Currently you can request head for the head-only asset, body for just the avatar body, or full for the entire avatar.</td>
  </tr>
</table>


## Request Default Avatars from BFG

Method: GET

Endpoint: [https://bitmoji.api.snapchat.com/bitmoji-for-games/default_avatar](https://bitmoji.api.snapchat.com/bitmoji-for-games/default_avatar)

Headers

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>Authorization</td>
    <td>Yes</td>
    <td>String</td>
    <td>Generated OAuth2.0 token from the successful authorization response from Login Kit.</td>
  </tr>
</table>


### Request Avatar

The following example shows how to fetch an avatar using the GET method:

<table>
  <tr>
    <td>Request</td>
  </tr>
  <tr>
    <td>curl -X GET 'https://bitmoji.api.snapchat.com/bitmoji-for-games/default_avatar?lod=3&color=d80030' \
-H 'Accept-Encoding: gzip, deflate' \
-H 'Authorization: Bearer {your_access_token}' </td>
  </tr>
</table>


Here’s the response if the service finds a model:

<table>
  <tr>
    <td>Response</td>
  </tr>
  <tr>
    <td>This endpoint will return a default 3d avatar model for the authenticated user.</td>
  </tr>
</table>


If there’s an error:

<table>
  <tr>
    <td>401 Unauthorized</td>
  </tr>
</table>


Ensure that your clientId for the application has been whitelisted by the Bitmoji team.

<table>
  <tr>
    <td>401 Unauthorized - Denied</td>
  </tr>
  <tr>
    <td>{
  "message": "invalid_token"
}</td>
  </tr>
</table>


This is an error from Snap Kit, showing the access token is not valid. Please check the bearer token under the authorization header.

<table>
  <tr>
    <td>500 Internal Server Error</td>
  </tr>
</table>


There is an error from the service. Please double check the request is valid and try again.

#### Parameters

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>lod</td>
    <td>No</td>
    <td>number</td>
    <td>The level of detail for a model. We currently have LOD 0 for the highest level of detail and LOD 3 for the level of detail seen in Snap Games.</td>
  </tr>
  <tr>
    <td>color</td>
    <td>No</td>
    <td>string</td>
    <td>The hex code of the color for the default avatar.</td>
  </tr>
</table>


If no color is specified, the avatar will be returned with a default color. The current valid list of colors are as follows: "d80030", "f65900", "ffba00", "43c93b", "00d5a0", "3461ef", "8936b6", "e50184", "f23c57", "ff8a00", "ffd800", "9ed900", "6dcfba", "0eadff", "a871ff", "ff65ad", "b20006", "cd4803", "d6a201", "156d10", "00a179", "243f96", "4b1d63", "9b055b", "f47b76", "ffb45f", "fde687", "c9e855", "a9ebe4", "7bd5fc", "cdadff", "ffa9d1"

## Request Test Avatar List from BFG

Method: GET

Endpoint: [https://bitmoji.api.snapchat.com/bitmoji-for-games/test_avatars](https://bitmoji.api.snapchat.com/bitmoji-for-games-staging/test_avatars)

Headers

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>Authorization</td>
    <td>Yes</td>
    <td>String</td>
    <td>Generated OAuth2.0 token from the successful authorization response from Login Kit.</td>
  </tr>
</table>


### Request Test Avatar List

The following example shows how to fetch a list of test avatars using the GET method:

<table>
  <tr>
    <td>Request</td>
  </tr>
  <tr>
    <td>curl -X GET \
https://bitmoji.api.snapchat.com/bitmoji-for-games/test_avatars
-H 'Authorization: Bearer <Insert user's OAuth2.0 access token>'</td>
  </tr>
</table>


Here’s the response:

<table>
  <tr>
    <td>Response</td>
  </tr>
  <tr>
    <td>This endpoint will return a list of UUIDs for the various test avatars that can be used in development.</td>
  </tr>
</table>


If there’s an error:

<table>
  <tr>
    <td>401 Unauthorized</td>
  </tr>
</table>


Ensure that your clientId for the application has been whitelisted by the Bitmoji team.

<table>
  <tr>
    <td>401 Unauthorized - Denied</td>
  </tr>
  <tr>
    <td>{
  "message": "invalid_token"
}</td>
  </tr>
</table>


This is an error from Snap-kit, showing the access token is not valid. Please check the bearer token under the authorization header.

<table>
  <tr>
    <td>500 Internal Server Error</td>
  </tr>
</table>


There is an error from the service. Please double check the request is valid and try again.

## Request Selfie URL

Method: POST

Endpoint: [https://api.snapkit.com/v1/me](https://api.snapkit.com/v1/me)

GraphQL Query: {me{bitmoji{selfie}}}

Headers

<table>
  <tr>
    <td>Field</td>
    <td>Required</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>Authorization</td>
    <td>Yes</td>
    <td>String</td>
    <td>Generated OAuth2.0 token from the successful authorization response from Login Kit.</td>
  </tr>
</table>


### Request Selfie URL

The following example shows how to fetch a player’s selfie URL using the POST method:

<table>
  <tr>
    <td>Request</td>
  </tr>
  <tr>
    <td>curl -X POST https://api.snapkit.com/v1/me \
--data "{\"query\": \"{me{bitmoji{selfie}}}\"}" \
-H "Content-Type: application/json" \
-H 'Authorization: Bearer {your_access_token}'</td>
  </tr>
</table>


Here’s the response:

<table>
  <tr>
    <td>Response</td>
  </tr>
  <tr>
    <td>{
   "data":{
      "me":{
         "Bitmoji":{
            "Selfie": "https://me.bitmoji.com/WgbfLWYyEwszKFF_VoU6xDkT8z3ulzPqIyl8jm-5IME"
         }
      }
   },
   "errors":[]
}</td>
  </tr>
</table>


If there’s an error:

<table>
  <tr>
    <td>401 Unauthorized - Denied</td>
  </tr>
  <tr>
    <td>{
  "message": "invalid_token"
}</td>
  </tr>
</table>


This is an error from Snap-kit, showing the access token is not valid. Please check the bearer token under the authorization header.

<table>
  <tr>
    <td>500 Internal Server Error</td>
  </tr>
</table>


There is an error from the service. Please double check the request is valid and try again.

# Troubleshooting

Any issues with development should be sent to the Bitmoji team at [games@bitmoji.com](mailto:games@bitmoji.com) or communicated in the Slack room that has been set up between your team and the Bitmoji team.

# Frequently asked questions

**How do I get access to BFG?**

Email the Bitmoji team at [games@bitmoji.com](mailto:games@bitmoji.com) with the development and production clientIds of the app you create inside the Snap Kit development portal.

