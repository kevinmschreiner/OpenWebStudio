var dflt = 'text/x-mssql';
var ceditor;

function cInit(id) {
    if (id == 'frmActionTemplate_Value') {
        var tempType = document.querySelector('select#frmActionTemplate_Type');
        if (tempType.value == 'Query-Query') {
            ceditor = CodeMirror.fromTextArea(document.getElementById(id), {
                lineNumbers: true,
                extraKeys: { "Tab": "autocomplete" },
                mode: dflt,
                indentWithTabs: false,
                indentUnit: 4,
                autoFocus: true,
                keymap: "sublime",
                theme: "dracula",
                matchBrackets: true,
                autoCloseBrackets: true,
                viewportMargin: Infinity
            });
        } else {
            ceditor = CodeMirror.fromTextArea(document.getElementById(id), {
                lineNumbers: true,
                extraKeys: { "`": "autocomplete" },
                mode: "htmlmixed",
                indentWithTabs: false,
                indentUnit: 4,
                autoFocus: true,
                keymap: "sublime",
                theme: "dracula",
                matchBrackets: true,
                autoCloseBrackets: true,
                viewportMargin: Infinity,
                profile: "html"
            });
            emmetCodeMirror(ceditor);
        }
    } else if (id == 'frmActionQuery_Query') {
        ceditor = CodeMirror.fromTextArea(document.getElementById(id), {
            lineNumbers: true,
            extraKeys: { "Tab": "autocomplete" },
            mode: dflt,
            indentWithTabs: false,
            indentUnit: 4,
            autoFocus: true,
            keymap: "sublime",
            theme: "dracula",
            matchBrackets: true,
            autoCloseBrackets: true,
            viewportMargin: Infinity
        });
    } else if (id == 'frmActionAssignment_Value') {
        ceditor = CodeMirror.fromTextArea(document.getElementById(id), {
            lineNumbers: true,
            extraKeys: { "Tab": "autocomplete" },
            mode: "text/x-puppet",
            indentWithTabs: false,
            indentUnit: 4,
            autoFocus: true,
            keymap: "sublime",
            theme: "dracula",
            matchBrackets: true,
            autoCloseBrackets: true,
            viewportMargin: Infinity
        });
    }
}