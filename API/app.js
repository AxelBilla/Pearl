import { createRequire } from "module";
import { fileURLToPath } from "url";
import { dirname } from "path";

import { Authentication } from "./Pearl/auth.js";

import { Messages } from "./Pearl/messages.js";
import { Newsletter } from "./Pearl/newsletter.js";
import { Administration } from "./Pearl/admin.js";

const require = createRequire(import.meta.url);
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);
const path = require("path");

const bcrypt = require("bcrypt");
const saltRounds = 13;

var express = require("express");
var app = express();

app.use(express.json())
app.use(express.urlencoded({ extended: true }))
app.use('/', express.static(path.join(__dirname + '/'))); // Sets it so the root path is always this current folder (hence why some files in "./views/" can directly access assets with the bare "/assets/" path)
app.use(function (req, res, next) {

  // Website you wish to allow to connect
  res.setHeader('Access-Control-Allow-Origin', '*');

  // Request methods you wish to allow
  res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE');

  // Request headers you wish to allow
  res.setHeader('Access-Control-Allow-Headers', 'X-Requested-With,content-type');
  
  next();
});

app.get('/', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Authentication.Information(auth) : "[401]: Unauthorized Access";
  res.json(exec);
})

app.get('/message', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Messages.Read() : "[401]: Unauthorized Access";
  res.json(exec);
})
app.post('/message', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Messages.Create(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})
app.put('/message', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Messages.Update(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})
app.delete('/message', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Messages.Delete(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})


app.get('/newsletter', async function(req, res){

  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Newsletter.Read() : "[401]: Unauthorized Access";
  res.json(exec);
})
app.post('/newsletter', async function(req, res){

  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);
  
  const exec = (has_access) ? await Newsletter.Create(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})
//app.put('/newsletter', async function(req, res){
//  let auth = Authentication.ToJSON(req.headers);
//  let has_access = await Authentication.HasAccess(auth);
//
//  const exec = (has_access) ? await Newsletter.Update(req.body, auth) : "[401]: Unauthorized Access";
//  res.json(exec);
//})
app.delete('/newsletter', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Newsletter.Delete(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})


// Admin 
app.get('/users', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let is_admin = (await Authentication.Information(auth)).is_admin;

  const exec = (is_admin) ? await Administration.Read.Users() : "[401]: Unauthorized Access";
  res.json(exec);
})

app.get('/admin', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let is_admin = (await Authentication.Information(auth)).is_admin;

  const exec = (is_admin) ? await Administration.Read.Admins() : "[401]: Unauthorized Access";
  res.json(exec);
})
app.post('/admin', async function(req, res){
  req.body.password = await bcrypt.hash(req.body.password, saltRounds);

  let auth = Authentication.ToJSON(req.headers);
  let is_admin = (await Authentication.Information(auth)).is_admin;

  const exec = (is_admin) ? await Administration.Create(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})
app.put('/admin', async function(req, res){
  req.body.password = await bcrypt.hash(req.body.password, saltRounds);

  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Administration.Update(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})
app.delete('/admin', async function(req, res){
  let auth = Authentication.ToJSON(req.headers);
  let has_access = await Authentication.HasAccess(auth);

  const exec = (has_access) ? await Administration.Delete(req.body, auth) : "[401]: Unauthorized Access";
  res.json(exec);
})



app.listen(9949);