function downloadFile(filename, content, mimeType) {
    const blob = new Blob([content], { type: mimeType });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
window.initializeDragAndDrop = (dropZone) => {
    if (!dropZone) return; // Ensure element exists

    dropZone.addEventListener("dragover", (event) => {
        event.preventDefault();
        dropZone.classList.add("dragover");
    });

    dropZone.addEventListener("dragleave", () => {
        dropZone.classList.remove("dragover");
    });

    dropZone.addEventListener("drop", (event) => {
        event.preventDefault();
        dropZone.classList.remove("dragover");

        let fileInput = dropZone.querySelector("input[type='file']");
        if (fileInput) {
            fileInput.files = event.dataTransfer.files;
            fileInput.dispatchEvent(new Event("change", { bubbles: true })); 
        }
    });
};

function refreshPage() {
    window.location.reload();
}

// Uploads the files currently sitting in the dropzone's <input type="file">
// via a plain HTTP POST, bypassing the Blazor Server SignalR circuit entirely
// (browserFile.OpenReadStream() reads over that circuit, which has a small
// message-size ceiling and silently kills the connection on large Excel files).
// Returns the server-saved temp file paths in the same order as the FileList,
// so callers can zip them up against e.GetMultipleFiles() by index.
window.uploadSelectedFiles = async (dropZone, uploadUrl) => {
    if (!dropZone) return [];
    const fileInput = dropZone.querySelector("input[type='file']");
    if (!fileInput || !fileInput.files || fileInput.files.length === 0) return [];

    const formData = new FormData();
    for (const file of fileInput.files) {
        formData.append("files", file);
    }

    const response = await fetch(uploadUrl, {
        method: "POST",
        body: formData
    });

    if (!response.ok) {
        throw new Error(`File upload failed with status ${response.status}`);
    }

    const result = await response.json();
    return result.paths;
};
