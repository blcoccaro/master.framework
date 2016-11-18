function Page() {
    this.Keyboard = new Keyboard;
    this.DropDownContainer = new DropDownContainer;
    this.JTableMessages = {
        serverCommunicationError: 'Falha ao tentar se comunicar com o servidor.',
        loadingMessage: 'Carregando...',
        noDataAvailable: 'Sem registros.',
        addNewRecord: 'Novo Registro',
        editRecord: 'Editar Registro',
        areYouSure: 'Aviso',
        deleteConfirmation: 'Esse registro será removido. Você tem certeza?',
        save: 'Salvar',
        saving: 'Salvando',
        cancel: 'Cancelar',
        deleteText: 'Remover',
        deleting: 'Removendo',
        error: 'Erro',
        close: 'Fechar',
        cannotLoadOptionsFor: 'Can not load options for field {0}',
        pagingInfo: 'Registros {0}-{1} de {2}',
        pageSizeChangeLabel: 'Qtd Registros',
        gotoPageLabel: 'Página',
        canNotDeletedRecords: 'Can not deleted {0} of {1} records!',
        deleteProggress: 'Removendo registro {0} de {1}, processando...',
        unauthorizedAccess: 'Acesso não autorizado.'
    };
}

function DropDownContainer() {
    this.List = new Array();
}

function DropDownList() {
    this.id = null,
    this.data = null
}

DropDownContainer.prototype.Add = function (id, data) {
    if (Page.DropDownContainer.List == null || Page.DropDownContainer.List == undefined) {
        Page.DropDownContainer.List = new Array();
    }
    var founded = false;
    for (i = 0; i < Page.DropDownContainer.List.length; i++) {
        if (Page.DropDownContainer.List[i].id == id) {
            Page.DropDownContainer.List[i].data = data;
            founded = true;
        }
    }
    if (!founded) {
        var add = new DropDownList();
        add.id = id;
        add.data = data;
        Page.DropDownContainer.List.push(add);
    }
}
DropDownContainer.prototype.Get = function (id) {
    if (Page.DropDownContainer.List != null || Page.DropDownContainer.List != undefined) {
        for (i = 0; i < Page.DropDownContainer.List.length; i++) {
            if (Page.DropDownContainer.List[i].id == id) {
                return Page.DropDownContainer.List[i].data;
            }
        }
    }
    return null;
}

function Keyboard() {
	this.Keys = {
		Enter: "return",
		Ctrln: "Ctrl+n",
		Ctrle: "Ctrl+e",
		Ctrlq: "Ctrl+q",
		Ctrls: "Ctrl+s",
		Ctrlr: "Ctrl+r",
		Ctrll: "Ctrl+l"
	}
}

function JTableEvents() {
    this.closeRequested = null,
    this.formClosed = null,
    this.formCreated = null,
    this.formSubmitting = null,
    this.loadingRecords = null,
    this.recordAdded = null,
    this.recordDeleted = null,
    this.recordsLoaded = null,
    this.recordUpdated = null,
    this.rowInserted = null,
    this.rowsRemoved = null,
    this.rowUpdated = null,
    this.selectionChanged = null
}

Page.prototype.SimpleAlert = function (text, isError) {
    var n = noty({
        text: text,
        layout: "center",
        modal: true,
        closeWith: ['click'],
        type: isError ? "error" : "alert"
        //closeWith: ['button'],
        //buttons: [{
        //    addClass: 'btn btn-primary', text: 'Ok', onClick: function ($noty) {
        //        $noty.close();
        //    }
        //}]
    });
}

Page.prototype.GoTo = function (url) {
    window.location = url;
};

Page.prototype.JoinToString = function (obj) {
    var checkeds = obj.map(function () { return $(this).val(); }).get().join(",");
    return checkeds;
};

Keyboard.prototype.Enter = function (obj, callback) {
    jQuery(obj).bind('keydown', Page.Keyboard.Keys.Enter, function (evt) {
        callback();
        return false;
    });
}
Keyboard.prototype.Any = function (key, obj, callback) {
    if (!Page.Keyboard.Enabled) { return; }
    jQuery(obj).bind('keydown', key, function (evt) {
        callback();
        return false;
    });
}


; (function ($) {
    "use strict";
    /*$.noty.defaults = {
        layout: 'top',
        theme: 'defaultTheme',
        type: 'alert',
        text: '',
        dismissQueue: true, // If you want to use queue feature set this true
        template: '<div class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>',
        animation: {
            open: { height: 'toggle' },
            close: { height: 'toggle' },
            easing: 'swing',
            speed: 500 // opening & closing animation speed
        },
        timeout: false, // delay for closing event. Set false for sticky notifications
        force: false, // adds notification to the beginning of queue when set to true
        modal: false,
        maxVisible: 5, // you can set max visible notification for dismissQueue true option
        closeWith: ['click'], // ['click', 'button', 'hover']
        callback: {
            onShow: function () { },
            afterShow: function () { },
            onClose: function () { },
            afterClose: function () { }
        },
        buttons: false // an array of buttons
    };*/

    $.fn.LoadGrid = function (args) {
    	args = (args == null || args == undefined) ? {} : args;
    	args.Parameters = (args.Parameters == null || args.Parameters == undefined) ? null : args.Parameters;
    	args.OnComplete = (args.OnComplete == null || args.OnComplete == undefined) ? null : args.OnComplete;
        this.jtable('load', args.Parameters, args.OnComplete);
    };

    $.fn.CreateGrid = function (args)
    {
        var fulloptions = args.options;

        fulloptions.actions = args.actions;
        fulloptions.fields = args.fields;
        if (args.methods != undefined && args.methods != null) {
        	fulloptions.closeRequested = args.methods.closeRequested;
        	fulloptions.formClosed = args.methods.formClosed;
        	fulloptions.formCreated = args.methods.formCreated;
        	fulloptions.formSubmitting = args.methods.formSubmitting;
        	fulloptions.loadingRecords = args.methods.loadingRecords;
        	fulloptions.recordAdded = args.methods.recordAdded;
        	fulloptions.recordDeleted = args.methods.recordDeleted;
        	fulloptions.recordsLoaded = args.methods.recordsLoaded;
        	fulloptions.recordUpdated = args.methods.recordUpdated;
        	fulloptions.rowInserted = args.methods.rowInserted;
        	fulloptions.rowsRemoved = args.methods.rowsRemoved;
        	fulloptions.rowUpdated = args.methods.rowUpdated;
        	fulloptions.selectionChanged = args.methods.selectionChanged;
        }
        fulloptions.messages = Page.JTableMessages;

        if (args.tableType == "child")
        {
            this.jtable("openChildTable", args.row, fulloptions, args.methods.completeCallback);
        }
        else {
            this.jtable(fulloptions);
        }
    }

    $.fn.Grid = function (args) {
        this.CreateGrid(args);
    };


    jQuery.migrateMute = false;

})(jQuery);

var Page = new Page();