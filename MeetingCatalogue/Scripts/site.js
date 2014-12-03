$.fn.editableWidget = function (options) {
    options = $.extend({
        id: 0,
        name: 'Test',
        endpoint: '/Meetings/UpdateTest',
        timestamp: 0
    }, options);

    var $self = $(this);
    var initialized = false;
    var $textarea;
    var $editArea;

    function showNotification(type, text, close) {
        var $alert = $('<div class="alert alert-' + type + '" role="alert">' + text + '</div>').appendTo($editArea.find('.notifications').empty());
        if (close) {
            setTimeout(function() {
                $alert.fadeOut();
            }, close * 1000);
        }
    }
    function enableEditing() {
        if (!initialized) {
            $editArea = $('<div class="editable-area"><div class="notifications"></div><div class="well new-content" style="display: none"></div><textarea></textarea><div class=""><button class="btn btn-success save-button"><span class="glyphicon glyphicon-ok"></span> Save</button><button class="btn btn-warning close-button pull-right"><span class="glyphicon glyphicon-remove"></span> Close</button></div></div>').insertBefore($self);
            $textarea = $editArea.find('textarea').ckeditor();

            $editArea.find('.save-button').click(save);
            $editArea.find('.close-button').click(function () {
                if (confirm('Are you sure you want to discard your changes?')) {
                    disableEditing(true);
                }
            });
        }

        $editArea.show();
        $textarea.val($self.find('.content').html());
        $self.hide();
    }

    function disableEditing(discardChanges) {
        $editArea.hide();
        if (!discardChanges) {
            $self.find('.content').html($textarea.val());
            updateNoContent();
        }
        $self.show();
    }

    function updateNoContent() {
        if ($.trim($self.find('.content').text()) == '') {
            $self.find('.no-content').show();
        } else {
            $self.find('.no-content').hide();
        }
    }

    function save() {
        var html = $textarea.val();
        $self.find('.save-button').prop('disabled', true);
        $.post(options.endpoint, { id: options.id, timestamp: options.timestamp, text: $textarea.val() })
        .done(function (data) {
            if (data.success) {
                //showNotification('success', 'Successfully saved ' + options.name);
                $editArea.find('.new-content').hide();
                disableEditing(false);
            } else if (data.text) {
                showNotification('warning', 'The ' + options.name + ' was updated while you were editing. Please review the new text below, update your text accordingly, and save again.');
                $editArea.find('.new-content').html(data.text).show();
            }
            options.timestamp = data.timestamp;
        })
        .error(function () {
            showNotification('danger', 'Couldn\'t update ' + options.name + ' because of a server error.', 5);
        })
        .always(function () {
            $self.find('.save-button').prop('disabled', false);
        })
    }

    // Initialization
    $self.attr('title', 'Click to edit');
    $self.append('<span class="edit-sign"><span class="glyphicon glyphicon-edit"></span></span>');
    $self.click(enableEditing);
    $('<p class="no-content"><em>No ' + options.name + ' yet.</em></p>').appendTo($self);
    updateNoContent();
}
