import React, { Component } from 'react';
import { Layout } from './Layout';
import axios from 'axios';

export class SendEmail extends Component {
    static displayName = Layout.name;
    constructor(props) {
        super(props);

        this.state = {
            email: "",
            filePath: ""
        };
    }

    sendEmail = async (e) => {
        let formData = new FormData();
        formData.append('email', this.state.email);
        formData.append('filePath', this.state.filePath);

        axios.post('http://localhost:26565/api/file/send-by-email', formData)
            .then(function (response) {
                window.location.href = "/";
            })
            .catch(function (error) {
                console.log(error);
            });
    }

    onChangeEmail = (e) => {
        this.setState({
            email: e.target.value
        });
    }

    componentDidMount() {
        if (this.props.location.query && this.props.location.query.filePath) {
            this.setState({
                filePath: this.props.location.query.filePath
            });
        }
    }

    render() {
        return (
            <div className="submit-form">
                <div>
                    <div className="form-group">
                        <label htmlFor="title">File</label>
                        <input
                            type="text"
                            className="form-control"
                            id="filePath"
                            disabled
                            value={this.state.filePath}
                            name="filePath" />
                    </div>
                    <div className="form-group">
                        <label htmlFor="title">Email address</label>
                        <input
                            type="text"
                            className="form-control"
                            id="email"
                            required
                            placeholder="Enter email address where file should be sent."
                            value={this.state.email}
                            onChange={this.onChangeEmail}
                            name="email" />
                    </div>
                    <button onClick={this.sendEmail} className="btn btn-success">
                        Save
                    </button>
                </div>
            </div>
        )
    }
}
