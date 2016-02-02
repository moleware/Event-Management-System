Date.prototype.addHours = function (h) {
    this.setHours(this.getHours() + h);
    return this;
}
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};
$(".datetimepicker").datetimepicker({
    language: 'en',
    pick12HourFormat: true,
    pickSeconds: false,
    sideBySide: true
});
function SynchStartAndEndTimePickers(start, end, move, adjustment) {
    if (move == 0)
    {
        if (start.data("DateTimePicker").date >= end.data("DateTimePicker").date)
        {
            start.data("DateTimePicker").setDate(new Date(end.data("DateTimePicker").date).addHours(0 - adjustment));
        }
    }
    else if (move == 1)
    {
        if (start.data("DateTimePicker").date >= end.data("DateTimePicker").date)
        {
            end.data("DateTimePicker").setDate(new Date(start.data("DateTimePicker").date).addHours(adjustment));
        }

    }
};
function SetData(data, el) {
    $(el).html(data);
}
function AppendData(data, el) {
    $(el).append(data);
}
function CreateAlert(message, type) {
    var html = "<div class=\"fade in clearfix alert alert-" + type + " alert-dismissable\" role=\"alert\">";
    html += "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>";
    html += "<div id=\"AlertMessage\">" + message + "</div>";
    html += "</div>";
    $("#Alert").html(html);
}
