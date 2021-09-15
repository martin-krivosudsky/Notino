import React, { Component } from 'react';
import { FileExplorer } from './FileExplorer'

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
        <FileExplorer />
    );
  }
}
