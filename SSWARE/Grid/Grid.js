const grids = [];

const column = function (name, tag, width, visible, editable) {

    this.name = name;
    this.tag = tag;
    this.width = width;
    this.visible = visible;
    this.editable = editable;

    this.setproperty = function (name, value) {
        this[name] = value;
        //alert(this[name]);
    };
};

const layout = function (columns) {

    this.option;
    this.columns = columns;
    this.settings;
};

const registerGrid = function (tag, layout, dataurl, uploadurl) {

    var sc = document.getElementsByTagName("script");
    var src;
    for (i = 0; i < sc.length; i++) {
        if (sc[i].src.match(/Grid\.js$/)) {
            src = sc[i].src.replace("Grid.js", "");
            break;
        }
    }

    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.type = "text/css";
    link.href = src + "Grid.css";
    link.media = "all";
    document.getElementsByTagName("head")[0].appendChild(link);

    //Member variable                
    var data;
    var preSelect;
    var preClass;

    //Events
    //Every event pops before executed ( except word 'after' included)
    //If you return false, process doesn't executed
    var onAfterSave = function () { };//result
    this.setOnAfterSave = function (gridEvent) {
        onAfterSave = gridEvent;
    };

    var onUpdateRow = function () { return true; };//row, col
    this.setOnUpdateRow = function (gridEvent) {
        onUpdateRow = gridEvent;
    };

    var onDeleteRow = function () { return true; };//row
    this.setOnDeleteRow = function (gridEvent) {
        onDeleteRow = gridEvent;
    };

    var onSelectRow = function () { return true; };//row, status
    this.setOnSelectRow = function (gridEvent) {
        onSelectRow = gridEvent;
    };

    var onClick = function () { };//row col
    this.setOnClick = function (gridEvent) {
        onClick = gridEvent;
    };

    var onDblClick = function () { };//row col
    this.setOnDblClick = function (gridEvent) {
        onDblClick = gridEvent;
    };

    //ToolScript
    this.reload = function () {
        data = undefined;
        gridAjax(null, datacallback, dataurl);
    };

    this.getValue = function (row, col) {
        if (Number.isInteger(row)) {
            row = data.table[row];
        }
        return row[col];
    };

    this.setValue = function (row, col, val) {
        if (Number.isInteger(row)) {
            row = data.table[row];
        }
        row[col] = val;
        row.td[col].innerText = val;
        row.updated = true;
    };

    //HtmlEvents
    this.cellClick = function (event) {

        if (event.target.col === undefined) return;
        onClick(event.target.row, event.target.col);

        var obj = event.target;

        if (preSelect !== undefined) {
            preSelect.selected = false;
            preSelect.classList.remove("col_selected");

            if (preClass !== undefined)
                preSelect.classList.add(preClass);
        }

        preSelect = obj;
        obj.selected = true;
        preClass = obj.classList[0];
        obj.classList.add("col_selected");
    };

    this.cellDblClick = function (event) {

        if (event.target.col === undefined) return;
        onDblClick(event.target.row, event.target.col);
        var obj = event.target;

        //start editing
        if (obj.col.editable) {
            if (obj.status === "none") {
                obj.status = "editing";
                obj.setAttribute("contenteditable", true);
                obj.setAttribute("class", "col_editing");
                obj.focus();
            }
        }
    };

    this.cellFocus = function (event) {

        if (event.target.col === undefined) return;

        var obj = event.target;
        console.log("focus");
    };

    this.cellFocusOut = function (event) {

        if (event.target.col === undefined) return;

        var obj = event.target;
        console.log("focusout");

        //end editing
        if (obj.status === "editing") {
            obj.status = "none";
            obj.setAttribute("contenteditable", false);
            updateRow(obj);
        }
    };

    this.cellKeyDown = function (event) {
        var obj = event.target;
        //console.log("keydown " + event.key);
    };

    this.cellKeyUp = function (event) {
        var obj = event.target;
        //console.log("keyup " + event.key);        
    };

    this.cellKeyPress = function (event) {
        var obj = event.target;
        console.log("keypress " + event.key);

        //end editing
        if (obj.status === "editing" && event.key === "Enter") {
            obj.status = "none";
            obj.setAttribute("contenteditable", false);
            updateRow(obj);
        }


    };

    //Member functions
    this.addRow = function (row) {

        row.deleted = false;
        row.td = {};

        data.table.push(row);

        var rownum = data.table.length;
        var table = document.getElementById(tag + "_table");

        var table_row = table.insertRow(rownum);
        table_row.row = row;
        table_row.setAttribute("id", tag + "_row_" + (rownum - 1));
        table_row.index = rownum - 1;
        row.tr = table_row;
        var cols = layout.columns;
        var td = table_row.insertCell(0);
        td.row = row;
        gridSetRowHeader(tag, td, src);

        for (var i = 0; i < cols.length; i++) {
            if (cols[i].visible) {
                td = table_row.insertCell(i);
                row.td[cols[i].name] = td;
                td.setAttribute("id", tag + "_" + (rownum - 1) + "_" + cols[i].name);
                td.innerText = row[cols[i].name];
                td.row = data.table[rownum - 1];
                td.col = cols[i];
                gridSetCellAttribute(td);
            }
        }

        row.inserted = true;

        return true;
    };

    this.deleteRow = function (row) {

        var tr = row.tr;

        if (!onDeleteRow(tr.row)) return false;

        tr.classList.toggle("row_deleted");

        if (tr.row.deleted === undefined ||
            tr.row.deleted === false) {
            tr.row.deleted = true;
        }
        else {
            tr.row.deleted = false;
        }
        return true;
    };

    this.selectRow = function (cb) {

        if (!onSelectRow(cb.target, cb.checked)) {
            cb.checked = !cb.checked;
            return false;
        }

        cb.target.selected = cb.selected;
        cb.target.tr.classList.toggle("row_selected");
        return true;

    };

    var updateRow = function (cell) {

        if (!onUpdateRow(cell.row, cell.col)) return false;

        if (cell.row.original === undefined) cell.row.original = {};

        if (cell.row.original[cell.col.name] === undefined)
            cell.row.original[cell.col.name] = cell.row[cell.col.name];

        cell.row[cell.col.name] = cell.innerHTML;
        cell.row.updated = "true";

        if (cell.row[cell.col.name] !== cell.row.original[cell.col.name]) {
            cell.setAttribute("class", "col_edited");
        } else {
            cell.setAttribute("class", "col_editable");
        }
        return true;
    };

    var loadGrid = function () {

        var htmlTable = document.createElement("table");

        htmlTable.addEventListener("click", function () { grids[tag].cellClick(event); });
        htmlTable.addEventListener("dblclick", function () { grids[tag].cellDblClick(event); });
        htmlTable.addEventListener("focus", function () { grids[tag].cellFocus(event); });
        htmlTable.addEventListener("focusout", function () { grids[tag].cellFocusOut(event); });
        htmlTable.addEventListener("keydown", function () { grids[tag].cellKeyDown(event); });
        htmlTable.addEventListener("keyup", function () { grids[tag].cellKeyUp(event); });
        htmlTable.addEventListener("keypress", function () { grids[tag].cellKeyPress(event); });

        htmlTable.style.width = "100%";
        htmlTable.setAttribute("id", tag + "_table");
        htmlTable.setAttribute("class", "grid");

        var tr, th, td;

        tr = document.createElement("tr");
        th = document.createElement("th");
        th.setAttribute("width", "50px");
        th.innerText = "-";
        tr.appendChild(th);

        var i, j;
        var cols = layout.columns;
        for (i = 0; i < cols.length; i++) {
            if (cols[i].visible) {
                th = document.createElement("th");
                th.setAttribute("id", cols[i].name);
                th.setAttribute("width", cols[i].width);
                th.innerText = cols[i].tag;
                tr.appendChild(th);
            }
        }
        htmlTable.appendChild(tr);

        for (i = 0; i < data.table.length; i++) {

            data.table[i].index = i;

            tr = document.createElement("tr");
            tr.setAttribute("id", tag + "_row_" + i);

            td = document.createElement("td");
            td.row = data.table[i];
            gridSetRowHeader(tag, td, src);

            tr.appendChild(td);
            data.table[i].tr = tr;
            data.table[i].td = {};

            for (j = 0; j < cols.length; j++) {
                if (cols[j].visible) {
                    td = document.createElement("td");
                    td.setAttribute("id", tag + "_" + i + "_" + cols[j].name);
                    td.row = data.table[i];
                    td.col = cols[j];

                    gridSetCellAttribute(td);
                    td.innerText = data.table[i][cols[j].name];
                    tr.appendChild(td);
                    data.table[i].td[cols[j].name] = td;
                }
            }
            tr.row = data.table[i];
            htmlTable.appendChild(tr);
        }

        var nodes = document.getElementById(tag);
        while (nodes.firstChild) {
            nodes.removeChild(nodes.firstChild);
        }
        document.getElementById(tag).appendChild(htmlTable);
    };

    this.save = function () {

        var table = data.table;
        var columns = layout.columns;
        var changes = [];
        var changeNode;
        var i, j;

        for (i = 0; i < table.length; i++) {
            changeNode = {};
            if (table[i].inserted && !table[i].deleted) {
                changeNode.iud = "i";
            }
            else if (table[i].updated) {

                for (j = 0; j < columns.length; j++) {
                    if (table[i][columns[j].name] !== table[i].original[columns[j].name]
                        && table[i].original[columns[j].name] !== undefined) {
                        changeNode.iud = "u";
                        break;
                    }
                }
            }
            else if (table[i].deleted && !table[i].inserted) {
                changeNode.iud = "d";
            }
            if (changeNode.iud) {
                for (j = 0; j < columns.length; j++) {
                    changeNode[columns[j].name] = table[i][columns[j].name];
                }
                changes.push(changeNode);
            }
        }

        var parameter = {};

        parameter.changes = JSON.stringify(changes);
        gridAjax(parameter, saveCallBack, uploadurl);
    };

    var dataCallBack = function (obj) {
        data = obj;
        loadGrid();
    };

    var saveCallBack = function (obj) {
        onAfterSave(obj);
    };

    //Working part
    grids[tag] = this;
    return gridAjax(null, dataCallBack, dataurl);
};

const gridAjax = function (obj, callback, url) {
    if (url === undefined) url = "/Form/Common/AjaxAuto.aspx";
    var xhr = new XMLHttpRequest();
    xhr.open('POST', url, true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.onload = function () {
        if (xhr.status === 200) {
            var obj;
            try {
                obj = JSON.parse(xhr.responseText);
            } catch (error) {
                obj = xhr.responseText;
            }
            callback(obj);
        }
        else {
            alert("Ajax Request Error. Status " + xhr.status);
        }
        return xhr.status;
    };
    xhr.send(encodeURI(gridParam(obj)));
};

const gridParam = function (object) {
    var encodedString = '';
    for (var prop in object) {
        if (object.hasOwnProperty(prop)) {
            if (encodedString.length > 0) {
                encodedString += '&';
            }
            encodedString += encodeURI(prop + '=' + object[prop]);
        }
    }
    return encodedString;
};

const setGrid = function (tag, layout, dataurl, uploadurl) {
    return registerGrid(tag, layout, dataurl, uploadurl);
};

const gridSetRowHeader = function (tag, cell, src) {

    var img = document.createElement("img");
    img.target = cell.row;
    img.setAttribute("src", src + "/Images/delete.png");
    img.addEventListener("click", function () { grids[tag].deleteRow(this.target); });

    var chk = document.createElement("input");
    chk.target = cell.row;
    chk.setAttribute("type", "checkbox");
    chk.addEventListener("click", function () { grids[tag].selectRow(this); });

    cell.appendChild(chk);
    cell.appendChild(img);
};

const gridSetCellAttribute = function (cell) {
    cell.status = "none";

    if (cell.col.editable) {
        cell.setAttribute("class", "col_editable");
    }
};