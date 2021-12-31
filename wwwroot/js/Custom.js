let index = 0;


function AddTag() {
    let tagEntry = document.getElementById("TagEntry");
    let searchResult = Search(tagEntry.value);

    if (searchResult == "") {
        swalCustom.fire({
            title: 'Empty!',
            text: 'You cannot enter an empty tag',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }

    else if (searchResult == "duplicate")
    {
        swalCustom.fire({
            title: 'Duplicate!',
            text: 'You can not enter a duplicate tag',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }

    else {
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }

    //Clear TagEntry control
    tagEntry.value = "";
}

function DeleteTag() {
    let tagCount = 1;
    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectedIndex == -1) {
        swalCustom.fire({
            title: 'Tag Not Selected!',
            text: 'You need to select a tag before deleting',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        return true;
    }

    while (tagCount > 0) {

        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
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

function Search(str) {
    if (str == "") {
        return "";
    }

    var tagsElement = document.getElementById('TagList');
    if (tagsElement) {
        let options = tagsElement.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value == str) {
                return "duplicate";
            }
        }
    }
}


