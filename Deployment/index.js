const PORT = 812;

const fs = require("file-system");

const http = require("http");
const express = require("express");
const app = express();

app.use(express.static("dist"));

app.listen(PORT, () => { });

app.get("/", (req, res) => {
    res.send("Website is online!")
});

console.log("Started website on port " + PORT)