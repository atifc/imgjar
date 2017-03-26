var previewTemplate = document.querySelector("#template").innerHTML;

var dropZone = new Dropzone("form#imgJarUploadForm", { url: "/upload", maxFiles: 1, acceptedFiles: "image/*", previewTemplate: previewTemplate });

dropZone.on("success", function (file, serverSaid) {
    dropZone.removeAllFiles(true);
    document.getElementById('uploadedUrl').value = "https://i.imgjar.co/jar/" + serverSaid.fileKey;
    document.getElementById('removalUrl').value = "https://imgjar.co/r/" + serverSaid.removalKey;

    var uploadResults = document.getElementById('upload-results');
    uploadResults.style.display = uploadResults.style.display === 'none' ? '' : 'none';
    document.getElementById('overlay').remove();
});

dropZone.on("error", function (file, serverSaid) {
    dropZone.removeAllFiles(true);
    var uploadResults = document.getElementById('upload-results-error');
    uploadResults.style.display = uploadResults.style.display === 'none' ? '' : 'none';
    document.getElementById('overlay').remove();
});

dropZone.on("addedfile", function () {
    var uploadResults = document.getElementById('upload-results');
    uploadResults.style.display = 'none';
});

dropZone.on("sending", function () {
    loading();
});

// loading overlay
var loading = function () {
    var overlay = document.createElement("div");
    overlay.id = "overlay";

    var overlayAnimation = document.createElement("img");
    overlayAnimation.id = "loading";
    overlayAnimation.src = "https://i.imgjar.co/jar/gaCF05HwYkq0u-oz2GCrcg.gif";

    overlay.appendChild(overlayAnimation);
    document.body.appendChild(overlay);
};