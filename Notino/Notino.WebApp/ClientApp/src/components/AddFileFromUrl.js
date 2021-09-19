import React, { Component } from 'react';
import { Layout } from './Layout';
import axios from 'axios';

export class AddFileFromUrl extends Component {
    static displayName = Layout.name;
    constructor(props) {
        super(props);

        this.state = {
            url: "",
            filePath: "",
            filename: ""
        };
    }

    uploadFile = async (e) => {
        let formData = new FormData();
        formData.append('url', this.state.url);
        formData.append('filePath', this.state.filePath);
        formData.append('filename', this.state.filename);

        axios.post('http://localhost:26565/api/file/upload-from-url', formData)
            .then(function (response) {
                window.location.href = "/";
            })
            .catch(function (error) {
                console.log(error);
            });
    }

    onChangePath = (e) => {
        this.setState({
            filePath: e.target.value
        });
    }

    onChangeUrl = (e) => {
        this.setState({
            url: e.target.value
        });
    }

    onChangeName = (e) => {
        this.setState({
            filename: e.target.value
        });
    }

    render() {
        return (
            <div className="submit-form">
                <div>
                    <div className="form-group">
                        <label htmlFor="title">File URL</label>
                        <input
                            type="text"
                            className="form-control"
                            id="url"
                            required
                            placeholder="Enter URL of file from where it should be downloaded"
                            value={this.state.fileUrl}
                            onChange={this.onChangeUrl}
                            name="url" />
                    </div>
                    <div className="form-group">
                        <label htmlFor="title">File name</label>
                        <input
                            type="text"
                            className="form-control"
                            id="filename"
                            required
                            placeholder="Enter name of the file"
                            value={this.state.filename}
                            onChange={this.onChangeName}
                            name="filename" />
                    </div>
                    <div className="form-group">
                        <label htmlFor="title">File path</label>
                        <input
                            type="text"
                            className="form-control"
                            id="filePath"
                            required
                            placeholder="Enter path where file should be saved on the server. If left blank file will be saved in root of storage. Example: 'Folder/Subfolder/'"
                            value={this.state.filePath}
                            onChange={this.onChangePath}
                            name="filePath" />
                    </div>
                    <button onClick={this.uploadFile} className="btn btn-success">
                        Save
                    </button>
                </div>
            </div>
        )
    }
}
