import React, { Component } from 'react';
import { Layout } from './Layout';
import axios from 'axios';

export class UploadFile extends Component {
    static displayName = Layout.name;
    constructor(props) {
        super(props);

        this.state = {
            file: null,
            filePath: null
        };
    }

    saveFile = (e) => {
        this.setState({ file: e.target.files[0] });
    }

    uploadFile = async (e) => {
        let formData = new FormData();
        formData.append('file', this.state.file);
        formData.append('path', this.state.filePath);

        axios.post('http://localhost:26565/api/file/upload', formData)
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

    render() {
        return (
            <div className="submit-form">
                <div>
                    <div className="form-group">
                        <label htmlFor="title">File path</label>
                        <input
                            type="text"
                            className="form-control"
                            id="filePath"
                            required
                            placeholder = "Enter path where file should be saved on the server. If left blank file will be saved in root of storage. Example: 'Folder/Subfolder/'"
                            value={this.state.filePath}
                            onChange={this.onChangePath}
                            name="filePath" />
                    </div>
                    <div className="form-group select-input">
                        <input type="file" onChange={this.saveFile} />
                    </div>
                    <button onClick={this.uploadFile} className="btn btn-success">
                        Save
                    </button>
                </div>
            </div>
        )
    }
}
