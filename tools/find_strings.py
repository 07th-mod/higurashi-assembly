from pathlib import Path
import re
import json
# import csv

# Try to match csharp strings
# - try to ignore escaped characters
# - try to ignore interpolated portions of strings
csharp_string_regex = re.compile(r"\"(((\{[^}]*\})|([^\\\"])|(\\.))*)\"")

plus_between_quotes = re.compile(r"\"\s*\+\s*\n\s*\"")

exclude_list_exact = [
    "LOCALWORK_NO_RESULT",
    "TipsMode",
    "SelectResult",
    "ChapterNumber",
    "LOnikakushiDay",
    "LTextFade",
    "GFlag_FirstPlay",
    "GFlag_GameClear",
    "GQsaveNum",
    "GOnikakushiDay",
    "GHighestChapter",
    "GMessageSpeed",
    "GAutoSpeed",
    "GAutoAdvSpeed",
    "GUsePrompts",
    "GSlowSkip",
    "GSkipUnread",
    "GClickDuringAuto",
    "GRightClickMenu",
    "GWindowOpacity",
    "GVoiceVolume",
    "GBGMVolume",
    "GSEVolume",
    "GCutVoiceOnClick",
    "GUseSystemSound",
    "GLanguage",
    "GVChie",
    "GVEiji",
    "GVKana",
    "GVKira",
    "GVMast",
    "GVMura",
    "GVRiho",
    "GVRmn_",
    "GVSari",
    "GVTika",
    "GVYayo",
    "GVOther",
    "GArtStyle",
    "GHideButtons",
    "GADVMode",
    "GLinemodeSp",
    "GCensor",
    "GEffectExtend",
    "GAltBGM",
    "GAltSE",
    "GAltBGMflow",
    "GAltSEflow",
    "GAltVoice",
    "GAltVoicePriority",
    "GCensorMaxNum",
    "GEffectExtendMaxNum",
    "GAltBGMflowMaxNum",
    "GAltSEflowMaxNum",
    "GMOD_SETTING_LOADER",
    "GFlagForTest1",
    "GFlagForTest2",
    "GFlagForTest3",
    "NVL_in_ADV",
    "LFlagMonitor",
    "DisableModHotkey",
    "GMOD_DEBUG_MODE",
    "GLipSync",
    "GVideoOpening",
    "GChoiceMode",
    "GHideCG",
    "GRyukishiMode",
    "GStretchBackgrounds",
    "GBackgroundSet",
    "GAudioSet",
    "GRyukishiMode43Aspect",
    "(",
    ")",
    " ",
    "\\n"
]

class LocalizedStringInfo:
    def __init__(self, id, fullstring, filename):
        self.id = id
        self.fullstring = fullstring
        self.filename = filename

class StringExtractor:
    def __init__(self, filename) -> None:
        self.count = 0
        self.filename = filename
        self.database = []

    def search_line(self, line):
        # If whole line is a comment, ignore it
        if line.strip().startswith('//'):
            return line

        newline = line

        line_comment = ''

        for match in csharp_string_regex.finditer(line):
            if self.count == 94:
                print('here')

            if not match:
                continue

            found_string = match.groups()[0]

            if found_string in exclude_list_exact:
                continue

            # Assume strings without any letters shouldn't be localized
            if not re.search("[a-zA-Z]", found_string):
                continue

            # If the whole string consists of a string interpolation, just skip it
            if '{' in found_string or '}' in found_string:
                continue

            # If "PlayerPrefs" in the line, ignore it as we probably don't want to localize it
            if 'PlayerPrefs' in line:
                continue

            # print(found_string)

            # Replace the string with a variable
            found_string_quoted = f'"{found_string}"'
            id = self.get_new_unique_id()
            replacement_string = f'Loc.{id}'

            newline = newline.replace(found_string_quoted, replacement_string)

            # Update the database with the found match
            self.database.append(LocalizedStringInfo(id, found_string, self.filename))

            # Update the comment at the end of the line (for developer to know what text was preivously there)
            line_comment += f"{found_string} | "


        # print(newline)

        line_comment = line_comment.strip()

        if line_comment != '':
            return f'{newline} //{line_comment.strip("| ")}'
        else:
            return newline


    
    def get_new_unique_id(self):
        temp = f"{self.filename}_{self.count}"
        self.count += 1
        return temp


# input = "MOD.Scripts.UI/MODMenuNormal.cs"
# input = "MOD.Scripts.UI/MODMenuResolution.cs"
# input = "MOD.Scripts.UI/MODMenuAudioOptions.cs"
# input = "MOD.Scripts.UI/MODMenuAudioSetup.cs"
# input = "MOD.Scripts.UI/MODMenuSupport.cs"
input = "MOD.Scripts.UI/MODMenu.cs"



filename = Path(input).stem
output_path = Path(input).with_suffix('.out.cs')
fragment_path = Path(input).with_suffix('.fragment.cs')
json_path = Path(input).with_suffix('.json')

with open(input, 'r', encoding='utf-8') as f:
    text = f.read()

    # Replace two strings attached by a plus
    text = plus_between_quotes.sub("", text)

    string_extractor = StringExtractor(filename)


    lines = []
    for line_no, line in enumerate(text.splitlines()):
        out_line = string_extractor.search_line(line)
        lines.append(out_line + '\n')

    with open(output_path, 'w', encoding='utf-8') as out_file:
        out_file.writelines(lines)

with open(fragment_path, 'w', encoding='utf-8') as fragment_file:
    for info in string_extractor.database:
        # fragment_file.write(f'addFallbackEntry("{info.id}", "{info.fullstring}");\n')
        fragment_file.write(f'public static string {info.id} => Get("{info.id}", "{info.fullstring}");\n')

for info in string_extractor.database:
    print(info.id, info.fullstring, info.filename) 

# Also write out a localization.json to be merged with the existing localization.json
all_chapters_data = {}
json_data = {
    "allChapters": all_chapters_data
}

for info in string_extractor.database:
    print(info)
    all_chapters_data[info.id] = {
        #"comment" : "No comment"
        "text": info.fullstring,
    }


with open(json_path, 'w', encoding='utf-8') as json_out:
    json_out.write(json.dumps(json_data, indent='\t', ensure_ascii=False))