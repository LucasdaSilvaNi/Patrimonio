//trim completo
String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, "");
},

//left trim
String.prototype.ltrim = function () {
    return this.replace(/^\s+/, "");
},

//right trim
String.prototype.rtrim = function () {
    return this.replace(/\s+$/, "");
}

