import { createRequire } from "module";
import { fileURLToPath } from "url";
import { dirname } from "path";

import { Messages } from "./Pearl/messages.js";
import { Authentication } from "./Pearl/auth.js";

const require = createRequire(import.meta.url);
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);
const path = require("path");

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

app.listen(9949);