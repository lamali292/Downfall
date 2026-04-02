@tool
extends EditorPlugin


func _enter_tree():
    add_custom_type(
        "MegaRichTextLabel", 
        "RichTextLabel", 
        load("res://addons/mega_text/MegaRichTextLabel.cs"), 
        preload("res://addons/mega_text/mega_richtextlabel_64.png"))
    add_custom_type(
        "MegaLabel", 
        "Label", 
        load("res://addons/mega_text/MegaLabel.cs"), 
        preload("res://addons/mega_text/mega_label.png"))


func _exit_tree():
    remove_custom_type("MegaRichTextLabel")
    remove_custom_type("MegaLabel")
