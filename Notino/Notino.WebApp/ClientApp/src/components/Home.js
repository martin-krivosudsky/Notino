import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const FileDownload = require('js-file-download');

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.state = {
            files: [],
            loading: true
        };
    }

    componentDidMount() {
        this.populateData();
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Full Path</th>
                        <th>Type</th>
                        <th>Size</th>
                        <th>Download</th>
                        <th>Send by Email</th>
                        <th>Convert</th>
                        <th>Delete</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.files.map(file =>
                        <tr key={file.path}>
                            <td>{file.name}</td>
                            <td>{file.path}</td>
                            <td>{file.fileType}</td>
                            <td>{(file.size / 1024).toFixed(2)}KB</td>
                            <td>
                                <Link to='/' onClick={() => this.download(file.path)}>
                                    Download
                                </Link>
                            </td>
                            <td>
                                <Link to={{ pathname: '/SendEmail', query: { filePath: file.path } }}>
                                    Send
                                </Link>
                            </td>
                            <td>
                                {/* This should be replaced with some dropdown when more conversion types are added. */}
                                {this.supportedConversion(file.fileType) &&
                                    <Link to='/' onClick={this.convert.bind(this, file.path, file.fileType)}>
                                    Convert to {this.mapConversions(file.fileType) }
                                    </Link>

                                }
                            </td>
                            <td>
                                <Link to='/' onClick={() => this.delete(file.path)}>
                                    Delete
                                </Link>
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>

        return (
            <div>
                <h1 id="tabelLabel" >Files</h1>
                {contents}
            </div>
        );
    }

    async populateData() {
        const response = await fetch('http://localhost:26565/api/file/get-all');
        const data = await response.json();
        this.setState({ files: data, loading: false });
    }

    download(path) {
        axios({
            url: 'http://localhost:26565/api/file/download',
            method: 'GET',
            responseType: 'blob',
            params: {
                path : path
            }
        })
            .then(function (response) {
                FileDownload(response.data, path.replace(/^.*[\\\/]/, ''));
            })
            .catch(function (error) {
                console.log(error);
            });
    }

    delete(path) {
        let that = this;

        axios.post('http://localhost:26565/api/file/delete', null, {
            params: {
                path: path
            }
        })
            .then(function (response) {
                that.setState({ loading: true });
                that.populateData();
            })
            .catch(function (error) {
                console.log(error);
            });
    }

    convert(path, type) {
        let that = this;

        axios.post('http://localhost:26565/api/file/convert',
            JSON.stringify({
                filePath: path,
                desiredType: type == 'JSON' ? 1 : 0
            }),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        )
            .then(function (response) {
                that.setState({ loading: true });
                that.populateData();
            })
            .catch(function (error) {
                console.log(error);
            });
    }

    supportedConversion(type) {
        return type == 'JSON' || type == 'XML';
    }

    mapConversions(type) {
        if (type == 'JSON')
            return 'XML';
        if (type == 'XML')
            return 'JSON';
    }
}
