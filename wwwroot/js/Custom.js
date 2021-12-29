﻿let index = 0;

function AddTag() {
    let tagEntry = document.getElementById("TagEntry");

    if (tagEntry.value == "" || tagEntry.value == null) {
        return;
    }

    else {
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;

        //Clear TagEntry control
        tagEntry.value = "";
    }

    console.log("test");
}

function DeleteTag() {
    let tagCount = 1;
    while (tagCount > 0) {
        let tagList = document.getElementById("TagList");
        let selectedIndex = tagList.selectedIndex;

        if (selectedIndex >= 0) {
            tagList.options[selectedIndex] = null;
            --tagCount;
        }

        else
            tagCount = 0;
        index--;
    }
}

$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
});

//Look for tagValues variable to see if it has data
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let i = 0; i < tagArray.length; i++) {
        //Load up or Replace the options that we have
        ReplaceTag(tagArray[i], i);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newOption = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newOption;
}