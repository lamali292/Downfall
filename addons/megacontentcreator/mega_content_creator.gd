@tool
extends EditorPlugin

const CARD_CLASS_TEMPLATE = "res://addons/megacontentcreator/content_templates/card_class_template.txt"
const POWER_CLASS_TEMPLATE = "res://addons/megacontentcreator/content_templates/power_class_template.txt"
const RELIC_CLASS_TEMPLATE = "res://addons/megacontentcreator/content_templates/relic_class_template.txt"
const MONSTER_CLASS_TEMPLATE = "res://addons/megacontentcreator/content_templates/monster_class_template.txt"

const CARD_LOC_FILE_PATH = "res://localization/eng/cards.json"
const POWERS_LOC_FILE_PATH = "res://localization/eng/powers.json"
const RELIC_LOC_FILE_PATH = "res://localization/eng/relics.json"
const MONSTER_LOC_FILE_PATH = "res://localization/eng/monsters.json"

const MONSTER_STATE_TEMPLATE = "        MoveState {0}State = new (\"{1}_MOVE\", {2}Move, TODOIntents);"
const MONSTER_ADD_STATE_TEMPLATE = "        states.Add({0}State);"
const MONSTER_MOVE_METHOD_TEMPLATE = "    private async Task {0}Move(IReadOnlyList<Creature> targets)\n    {\n    }"

var dock: Control

var card_tab: Button
var power_tab: Button
var relic_tab: Button
var monster_tab: Button

var card_container: Control
var power_container: Control
var relic_container: Control
var monster_container: Control

var card_name_field: TextEdit
var card_needs_power_checkbox: CheckBox
var create_card_button: Button
var deprecate_card_button: Button
var deprecate_power_button: Button
var deprecate_relic_button: Button

var power_name_field: TextEdit
var create_power_button: Button

var relic_name_field: TextEdit
var create_relic_button: Button

var monster_name_field: TextEdit
var monster_moves_field: TextEdit
var create_monster_button: Button

func _enter_tree():
    dock = load("res://addons/megacontentcreator/content_creator_dock.tscn").instantiate()
    add_control_to_dock(DOCK_SLOT_LEFT_UR, dock)
    setup_dock()

func setup_dock():
    card_tab = dock.get_node("%CardTab")
    power_tab = dock.get_node("%PowerTab")
    relic_tab = dock.get_node("%RelicTab")
    monster_tab = dock.get_node("%MonsterTab")

    card_container = dock.get_node("%CardContainer")
    power_container = dock.get_node("%PowerContainer")
    relic_container = dock.get_node("%RelicContainer")
    monster_container = dock.get_node("%MonsterContainer")

    card_needs_power_checkbox = dock.get_node("%NeedsPowerCheckBox")
    card_name_field = dock.get_node("%CardNameField")
    create_card_button = dock.get_node("%CreateCardButton")
    power_name_field = dock.get_node("%PowerNameField")
    create_power_button = dock.get_node("%CreatePowerButton")
    relic_name_field = dock.get_node("%RelicNameField")
    create_relic_button = dock.get_node("%CreateRelicButton")
    monster_name_field = dock.get_node("%MonsterNameField")
    monster_moves_field = dock.get_node("%MonsterMovesField")
    create_monster_button = dock.get_node("%CreateMonsterButton")

    deprecate_card_button = dock.get_node("%DeprecateCardButton")
    deprecate_power_button = dock.get_node("%DeprecatePowerButton")
    deprecate_relic_button = dock.get_node("%DeprecateRelicButton")

    create_card_button.button_down.connect( func(): generate_card(card_name_field.text, card_needs_power_checkbox.button_pressed))
    create_power_button.button_down.connect( func(): generate_power(power_name_field.text, false))
    create_relic_button.button_down.connect( func(): generate_relic(relic_name_field.text))
    create_monster_button.button_down.connect( func(): generate_monster(monster_name_field.text))

    deprecate_card_button.button_down.connect( func(): deprecate_card(card_name_field.text))
    deprecate_power_button.button_down.connect( func(): deprecate_power(power_name_field.text))
    deprecate_relic_button.button_down.connect( func(): deprecate_relic(relic_name_field.text))

    card_tab.button_down.connect( func(): switch_tab(card_container))
    power_tab.button_down.connect( func(): switch_tab(power_container))
    relic_tab.button_down.connect( func(): switch_tab(relic_container))
    monster_tab.button_down.connect( func(): switch_tab(monster_container))

    switch_tab(card_container)

func switch_tab(container: Control):
    card_container.visible = false
    power_container.visible = false
    relic_container.visible = false
    monster_container.visible = false

    container.visible = true

func generate_card(card_name: String, needs_power: bool):
    print("creating " + card_name + "...")

    var template = FileAccess.open(CARD_CLASS_TEMPLATE, FileAccess.READ)
    if template == null:
        print("Error: Could not open card template file")
        return
    var template_content = template.get_as_text()
    template_content = template_content.replace("{0}", cleaned_string(card_name))
    template.close()

    var new_cs_file = FileAccess.open("res://src/Core/Models/Cards/" + cleaned_string(card_name) + ".cs", FileAccess.WRITE)
    if new_cs_file == null:
        print("Error: Could not create card file")
        return
    new_cs_file.store_string(template_content)
    new_cs_file.close()

    var new_loc_header = "\"" + snake_case(cleaned_string(card_name)).to_upper()

    var lines = []
    var file = FileAccess.open(CARD_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open card localization file")
        return
    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1:
            lines.append(line)
    file.close()

    lines.append("  " + new_loc_header + ".title\": \"" + card_name + "\",")
    lines.append("  " + new_loc_header + ".description\": \"TODO\",")
    lines.sort()

    file = FileAccess.open(CARD_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write card localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()

    print("finished")

    if needs_power:
        generate_power(card_name + " Power", true)

func deprecate_card(card_name: String):
    print("deprecating " + card_name + "...")


    if !card_name.contains(":"):
        push_error("Card name must include pool (e.g., 'ironclad:Strike')")
        return


    var pool: String = card_name.split(":")[0]
    card_name = card_name.split(":")[1]


    var dir: DirAccess = DirAccess.open("res://")

    var fileName: String = "src/Core/Models/Cards/" + cleaned_string(card_name) + ".cs"
    if dir.file_exists(fileName):
        dir.remove(fileName)
        dir.remove(fileName + ".uid")


    var loc_header = "\"" + snake_case(cleaned_string(card_name)).to_upper()

    var lines = []
    var file = FileAccess.open(CARD_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open card localization file")
        return

    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1 and !line.begins_with("  " + loc_header):
            lines.append(line)
    file.close()
    lines.sort()

    file = FileAccess.open(CARD_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write card localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()


    var portrait: String = "images/packed/card_portraits/" + pool + "/" + snake_case(cleaned_string(card_name)) + ".png"
    if dir.file_exists(portrait):
        dir.remove(portrait)
        dir.remove(portrait + ".import")


    var beta_portrait: String = "images/packed/card_portraits/" + pool + "/beta/" + snake_case(cleaned_string(card_name)) + ".png"
    if dir.file_exists(beta_portrait):
        dir.remove(beta_portrait)
        dir.remove(beta_portrait + ".import")

    print("finished")


func generate_power(power_name: String, from_card: bool):
    print("creating " + power_name + "...")

    var template = FileAccess.open(POWER_CLASS_TEMPLATE, FileAccess.READ)
    if template == null:
        print("Error: Could not open power template file")
        return
    var template_content = template.get_as_text()
    template_content = template_content.replace("{0}", cleaned_string(power_name))
    template.close()

    var new_cs_file = FileAccess.open("res://src/Core/Models/Powers/" + cleaned_string(power_name) + ".cs", FileAccess.WRITE)
    if new_cs_file == null:
        print("Error: Could not create power file")
        return
    new_cs_file.store_string(template_content)
    new_cs_file.close()

    var new_loc_header = "\"" + snake_case(cleaned_string(power_name)).to_upper()

    var lines = []
    var file = FileAccess.open(POWERS_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open power localization file")
        return
    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1:
            lines.append(line)
    file.close()

    var real_power_name = power_name

    if from_card:
        real_power_name = real_power_name.replace(" Power", "")

    lines.append("  " + new_loc_header + ".title\": \"" + real_power_name + "\",")
    lines.append("  " + new_loc_header + ".description\": \"TODO\",")
    lines.append("  " + new_loc_header + ".smartDescription\": \"TODO\",")
    lines.sort()

    file = FileAccess.open(POWERS_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write power localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()

    print("finished")


func deprecate_power(power_name: String):
    print("deprecating " + power_name + "...")

    var dir: DirAccess = DirAccess.open("res://")

    var fileName: String = "src/Core/Models/Powers/" + cleaned_string(power_name) + ".cs"
    if dir.file_exists(fileName):
        dir.remove(fileName)
        dir.remove(fileName + ".uid")


    var loc_header = "\"" + snake_case(cleaned_string(power_name)).to_upper()

    var lines = []
    var file = FileAccess.open(POWERS_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open power localization file")
        return

    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1 and !line.begins_with("  " + loc_header):
            lines.append(line)
    file.close()
    lines.sort()

    file = FileAccess.open(POWERS_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write power localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()



    var icon: String = "images/powers/" + snake_case(cleaned_string(power_name)) + ".png"
    if dir.file_exists(icon):
        dir.remove(icon)
        dir.remove(icon + ".import")

    var beta_icon: String = "images/powers/beta/" + snake_case(cleaned_string(power_name)) + ".png"
    if dir.file_exists(beta_icon):
        dir.remove(beta_icon)
        dir.remove(beta_icon + ".import")
    print("finished")


func generate_relic(relic_name: String):
    print("creating " + relic_name + "...")

    var template = FileAccess.open(RELIC_CLASS_TEMPLATE, FileAccess.READ)
    if template == null:
        print("Error: Could not open relic template file")
        return
    var template_content = template.get_as_text()
    template_content = template_content.replace("{0}", cleaned_string(relic_name))
    template.close()

    var new_cs_file = FileAccess.open("res://src/Core/Models/Relics/" + cleaned_string(relic_name) + ".cs", FileAccess.WRITE)
    if new_cs_file == null:
        print("Error: Could not create relic file")
        return
    new_cs_file.store_string(template_content)
    new_cs_file.close()

    var new_loc_header = "\"" + snake_case(cleaned_string(relic_name)).to_upper()

    var lines = []
    var file = FileAccess.open(RELIC_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open relic localization file")
        return
    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1:
            lines.append(line)
    file.close()

    lines.append("  " + new_loc_header + ".title\": \"" + relic_name + "\",")
    lines.append("  " + new_loc_header + ".flavor\": \"PLACEHOLDER\",")
    lines.append("  " + new_loc_header + ".description\": \"TODO\",")
    lines.sort()

    file = FileAccess.open(RELIC_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write relic localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()

    print("finished")

func deprecate_relic(relic_name: String):
    print("deprecating " + relic_name + "...")

    var dir: DirAccess = DirAccess.open("res://")

    var fileName: String = "src/Core/Models/Relics/" + cleaned_string(relic_name) + ".cs"
    if dir.file_exists(fileName):
        dir.remove(fileName)
        dir.remove(fileName + ".uid")


    var loc_header = "\"" + snake_case(cleaned_string(relic_name)).to_upper()

    var lines = []
    var file = FileAccess.open(RELIC_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open relic localization file")
        return

    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1 and !line.begins_with("  " + loc_header):
            lines.append(line)
    file.close()
    lines.sort()

    file = FileAccess.open(RELIC_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write card localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()


    var icon: String = "images/relics/" + snake_case(cleaned_string(relic_name)) + ".png"
    if dir.file_exists(icon):
        dir.remove(icon)
        dir.remove(icon + ".import")

    var beta_icon: String = "images/relics/beta/" + snake_case(cleaned_string(relic_name)) + ".png"
    if dir.file_exists(beta_icon):
        dir.remove(beta_icon)
        dir.remove(beta_icon + ".import")

    var small_icon: String = "images/packed/relic_small/" + snake_case(cleaned_string(relic_name))
    if dir.file_exists(small_icon + "_small.png"):
        print(small_icon + "_small.png")
        dir.remove(small_icon + "_small.png")
        dir.remove(small_icon + "_small.png.import")
        dir.remove(small_icon + "_small_stroke.png")
        dir.remove(small_icon + "_small_stroke.png.import")
    print("finished")

func generate_monster(monster_name: String):
    print("creating " + monster_name + "...")

    var template = FileAccess.open(MONSTER_CLASS_TEMPLATE, FileAccess.READ)
    if template == null:
        print("Error: Could not open monster template file")
        return
    var template_content = template.get_as_text()
    template.close()

    template_content = template_content.replace("{0}", cleaned_string(monster_name))

    var move_methods = ""
    var move_states = ""
    var state_adds = ""

    var monster_moves = monster_moves_field.text.split(",")
    for i in range(monster_moves.size()):
        monster_moves[i] = monster_moves[i].strip_edges()

    for monster_move in monster_moves:
        move_methods += MONSTER_MOVE_METHOD_TEMPLATE.replace("{0}", cleaned_string(monster_move)) + "\n\n"

        var move_state = MONSTER_STATE_TEMPLATE

        var state_name = cleaned_string(monster_move)
        if state_name.length() > 0:
            state_name = state_name[0].to_lower() + state_name.right(-1)
        else:
            state_name = ""

        move_state = move_state.replace("{0}", state_name)
        move_state = move_state.replace("{1}", snake_case(cleaned_string(monster_move)).to_upper())
        move_state = move_state.replace("{2}", cleaned_string(monster_move))

        move_states += move_state + "\n"
        state_adds += MONSTER_ADD_STATE_TEMPLATE.replace("{0}", state_name) + "\n"

    template_content = template_content.replace("{1}", move_states)
    template_content = template_content.replace("{2}", state_adds)
    template_content = template_content.replace("{3}", move_methods)

    var new_cs_file = FileAccess.open("res://src/Core/Models/Monsters/" + cleaned_string(monster_name) + ".cs", FileAccess.WRITE)
    if new_cs_file == null:
        print("Error: Could not create monster file")
        return
    new_cs_file.store_string(template_content)
    new_cs_file.close()

    var new_loc_header = "\"" + snake_case(cleaned_string(monster_name)).to_upper()

    var lines = []
    var file = FileAccess.open(MONSTER_LOC_FILE_PATH, FileAccess.READ)
    if file == null:
        print("Error: Could not open monster localization file")
        return
    while not file.eof_reached():
        var line = file.get_line()
        if line.length() > 1:
            lines.append(line)
    file.close()

    lines.append("  " + new_loc_header + ".name\": \"" + monster_name + "\",")
    for monster_move in monster_moves:
        lines.append("  " + new_loc_header + ".moves." + snake_case(cleaned_string(monster_move)).to_upper() + ".title\": \"" + monster_move + "\",")
    lines.sort()

    file = FileAccess.open(MONSTER_LOC_FILE_PATH, FileAccess.WRITE)
    if file == null:
        print("Error: Could not write monster localization file")
        return
    file.store_line("{")
    for line in lines:
        file.store_line(line)
    file.store_line("}")
    file.close()

    print("finished")

func cleaned_string(input: String) -> String:
    var regex = RegEx.new()
    regex.compile("[^a-zA-Z0-9]")
    return regex.sub(input, "", true)

func snake_case(input: String) -> String:
    var result = ""
    for i in range(input.length()):
        var c = input[i]
        if c >= "A" and c <= "Z":
            if i > 0:
                result += "_"
            result += c.to_lower()
        else:
            result += c
    return result

func _exit_tree():
    remove_control_from_docks(dock)
    dock.queue_free()
