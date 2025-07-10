import logo from './logo.svg';
import './App.css';
import React from 'react';
//import WorkerList from './components/Workerlist';
//import AddWorkerForm from './components/AddWorkerForm';
import WorkCalendar from './components/WorkCalendar';

function App() {
  return (
    <div className="App" style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <h1>WorkLog App</h1>
      <WorkCalendar />
    </div>
  );
}

export default App;
