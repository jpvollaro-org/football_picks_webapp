import { useState } from 'react';
import useKeyPress from '../hooks/use-keyPress';
import useHttps from '../hooks/use-https';

const AdminComponent = (props) => {
	const { isLoading, error, sendRequestToFetch: DownloadFileNow } = useHttps();
	const [adminState, setAdminState] = useState(false);

	const toggleAdminState = (event) => {
		setAdminState(adminState !== true);
	}

	const downloadSpreadsheet = (event) => {
		var urlString = "api/ReactProgram/DownloadFile";
		DownloadFileNow({ url: urlString }, transformFile);
	}

	const base64ToByteArray = (base64) => {
		var binary_string = window.atob(base64);
		var len = binary_string.length;
		var bytes = new Uint8Array(len);
		for (var i = 0; i < len; i++) {
			bytes[i] = binary_string.charCodeAt(i);
		}
		return bytes.buffer;
	}

	const transformFile = ((response) => {
		var byteArray = base64ToByteArray(response.strData);
		var blob = new Blob([byteArray], { type: "application/octet-stream" });
		var link = document.createElement('a');
		link.href = window.URL.createObjectURL(blob);
		link.download = response.fileName;
		link.click();
	});

	useKeyPress(['~'], toggleAdminState);
	let adminComponentText;
	if (adminState) {
		adminComponentText =
			<div>
				<button onClick={downloadSpreadsheet}>Download Spreadsheet</button>
			</div>;
	}
	else {
		adminComponentText = <div />;
	}
	return adminComponentText;
};

export default AdminComponent;