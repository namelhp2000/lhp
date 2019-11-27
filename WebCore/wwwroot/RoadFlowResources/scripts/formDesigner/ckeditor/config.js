/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.extraPlugins = 'tableresizerowandcolumn,rf_attribute,rf_text,rf_textarea,rf_select,rf_radio,rf_checkbox,rf_hidden,rf_button,rf_html,rf_label,rf_files,rf_datetime,rf_organize,rf_lrselect,rf_subtable,rf_serialnumber,rf_selectdiv,rf_signature,rf_datatable,rf_script,rf_save,rf_publish';

};

//删除控件
CKEDITOR.rf_remove = function (editor) {
    if(null == currentSelectEditorElement) {
        currentSelectEditorElement = editor.getSelection().getStartElement();
    }
    var element = CKEDITOR.dom.element.get(currentSelectEditorElement);
    var parent = element.getParent();
    if(parent && "td" == parent.getName().toLowerCase()) {
        parent.setHtml("&nbsp;");
    } else {
        element.remove();
    }
    currentSelectEditorElement = null;
}