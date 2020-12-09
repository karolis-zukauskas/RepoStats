import React, { Component } from 'react';
import { Route } from 'react-router';
import { Home } from './components/Home';
import { Settings } from './components/Settings';
import { NavMenu } from './components/NavMenu';

import "./App.css";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <div class="flex-container">
        <NavMenu />
        <Route exact path='/' component={Home} />
        <Route path='/settings' component={Settings} />
      </div>
    );
  }
}
