
<h1 align="center">Pearl - The Anything App</h1>
<br>
<p align="center">
  <em>Pearl is an application meant to simplify the access to, and production of,
    <br>private communication networks and desktop apps.</em>
  <br>
  <br>
  <a href="https://github.com/AxelBilla/Pearl/releases/"><img src="https://img.shields.io/github/release/AxelBilla/Pearl?include_prereleases=&sort=semver&color=blue" alt="GitHub release"></a>
  <a href="#license"><img src="https://img.shields.io/badge/License-CC_BY--NC--ND_4.0-blue" alt="License"></a>
  <a href="https://github.com/AxelBilla/Pearl/issues"><img src="https://img.shields.io/github/issues/AxelBilla/Pearl" alt="issues - Pearl"></a>
  <br>
  <br>
  <br>
  The use of a Reverse Proxy is recommended.
</p>
<h1 align="center">Documentation</h1>
<br>
<h2 align="center">API - Administration</h2>
<p align="right">? = Optional</p>

### `GET USERS` : `GET /users`
    {}
  <p align="center">Display a list of all users.</p>
  <br>
<h2 align="center"></h2>

### `GET ADMINS` : `GET /admin`
    {}
  <p align="center">Display a list of all admins.</p>
  <br>
<h2 align="center"></h2>

### `CREATE USER` : `POST /admin`
    {
      "username":"[NAME]",
      "password":"[PASSWORD]",
      "is_admin": [true/false]
    }
  <p align="center">Creation of a new user, with or without admin permissions.</p>
  <br>
<h2 align="center"></h2>

### `UPDATE USER` : `PUT /admin`
    {
      ? "id":"[ID]",
      "username":"[NAME]",
      "password":"[PASSWORD]",
      "is_admin": [true/false]
    }
  <p align="center">Update of an existing user.<br>(Username can only be changed by including ID)</p>
  <br>
<h2 align="center"></h2>

### `DELETE USER` : `DELETE /admin`
    {
      ? "id":"[ID]",
      "username":"[NAME]"
    }
  <p align="center">Delete an existing user.<br>(ID can be replaced by username)</p>
  <br>
<h2 align="center"></h2>
