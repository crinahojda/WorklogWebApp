import React, { useState, useEffect } from 'react';
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';
import API from '../services/api';

const WorkCalendar = () => {
  const [selectedDate, setSelectedDate] = useState(null);
  const [selectedWorkerId, setSelectedWorkerId] = useState('');
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [description, setDescription] = useState('');
  const [workers, setWorkers] = useState([]);
  const [workTimes, setWorkTimes] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [workTimeToEdit, setWorkTimeToEdit] = useState(null);

  useEffect(() => {
    const fetchWorkers = async () => {
      try {
        const response = await API.get('/Worker/GetWorkers');
        setWorkers(response.data);
        if (response.data.length > 0) {
          setSelectedWorkerId(response.data[0].Id);
        }
      } catch (error) {
        console.error('Eroare la preluarea muncitorilor:', error);
      }
    };

    fetchWorkers();
  }, []);

  useEffect(() => {
    fetchWorkTimes();
  }, [selectedWorkerId]);

  const fetchWorkTimes = async () => {
    if (!selectedWorkerId) return;
    try {
      const response = await API.get(`/WorkTime/GetWorkTimesByWorker?workerId=${selectedWorkerId}`);
      setWorkTimes(response.data);
    } catch (error) {
      console.error('Eroare la preluarea timpilor de lucru:', error);
    }
  };

  const handleSubmit = async () => {
    if (!selectedWorkerId || !startTime || !endTime || !selectedDate) {
      alert('Selectează muncitorul, data și introdu ora de început și de sfârșit!');
      return;
    }

    const startDateTime = new Date(selectedDate);
    const endDateTime = new Date(selectedDate);

    const [startHour, startMinute] = startTime.split(':');
    const [endHour, endMinute] = endTime.split(':');

    startDateTime.setHours(parseInt(startHour), parseInt(startMinute), 0, 0);
    startDateTime.setMinutes(startDateTime.getMinutes() - startDateTime.getTimezoneOffset());

    endDateTime.setHours(parseInt(endHour), parseInt(endMinute), 0, 0);
    endDateTime.setMinutes(endDateTime.getMinutes() - endDateTime.getTimezoneOffset());

    const payload = {
      workerGuid: selectedWorkerId,
      startDate: startDateTime,
      endDate: endDateTime,
      description: description || null,
    };

    try {
      if (editMode && workTimeToEdit) {
        await API.put(`/WorkTime/UpdateWorkTime?id=${workTimeToEdit.id}`, payload);
        alert('Timpul de lucru a fost modificat cu succes!');
      } else {
        await API.post('/WorkTime/AddWorkTimeWithDetails', payload);
        alert('Timpul de lucru a fost salvat cu succes!');
      }
      setStartTime('');
      setEndTime('');
      setDescription('');
      setShowForm(false);
      setEditMode(false);
      setWorkTimeToEdit(null);
      fetchWorkTimes(); // Refresh work times without page reload
    } catch (error) {
      console.error('Eroare la salvarea timpului de lucru:', error);
    }
  };

  const handleEdit = (wt) => {
    const start = new Date(wt.startDate);
    const end = new Date(wt.endDate);
    setStartTime(start.toTimeString().substring(0, 5));
    setEndTime(end.toTimeString().substring(0, 5));
    setDescription(wt.description || '');
    setShowForm(true);
    setEditMode(true);
    setWorkTimeToEdit(wt);
  };

  const handleDelete = async (id) => {
    try {
      await API.delete(`/WorkTime/DeleteWorkTime?id=${id}`);
      alert('Timpul de lucru a fost șters.');
      fetchWorkTimes(); // Refresh work times after deletion
    } catch (error) {
      console.error('Eroare la ștergerea timpului:', error);
    }
  };

  const filteredWorkTimes = selectedDate
    ? workTimes.filter((wt) => {
        const start = new Date(wt.startDate);
        return (
          start.getFullYear() === selectedDate.getFullYear() &&
          start.getMonth() === selectedDate.getMonth() &&
          start.getDate() === selectedDate.getDate()
        );
      })
    : [];

  return (
    <div style={{ textAlign: 'center' }}>
      <h1>WorkLog App</h1>
      <h2>Calendar Zile Muncă</h2>

      <select
        value={selectedWorkerId}
        onChange={(e) => {
          setSelectedWorkerId(e.target.value);
          setStartTime('');
          setEndTime('');
          setDescription('');
          setWorkTimes([]);
        }}
        style={{ marginBottom: '10px', padding: '5px' }}
      >
        <option value="">-- Selectează muncitor --</option>
        {workers.map((worker) => (
          <option key={worker.Id} value={worker.Id}>
            {worker.Name} ({worker.Position})
          </option>
        ))}
      </select>

      <Calendar onChange={setSelectedDate} value={selectedDate} />

      {selectedDate && (
        <div style={{ marginTop: '30px' }}>
          <h3>Timp lucrat în {selectedDate.toLocaleDateString()}</h3>
          {filteredWorkTimes.length === 0 ? (
            <p>Nu există timpi înregistrați pentru această zi.</p>
          ) : (
            <ul>
              {filteredWorkTimes.map((wt) => (
                <li key={wt.id}>
                  {new Date(wt.startDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} -
                  {new Date(wt.endDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} :{' '}
                  {wt.description || 'fără descriere'}{' '}
                  <button onClick={() => handleEdit(wt)}>Modifică</button>
                  <button onClick={() => handleDelete(wt.id)}>Șterge</button>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      <button onClick={() => {
        setShowForm(!showForm);
        setEditMode(false);
        setWorkTimeToEdit(null);
      }} style={{ marginTop: '10px' }}>
        {showForm ? 'Anulează' : 'Adaugă timp de lucru'}
      </button>

      {showForm && (
        <div style={{ marginTop: '20px' }}>
          <div>
            <label>Ora de începere: </label>
            <input
              type="time"
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
            />
          </div>

          <div>
            <label>Ora de final: </label>
            <input
              type="time"
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
            />
          </div>

          <div>
            <label>Descriere activitate: </label>
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="ex: Curățenie, montaj, etc. (opțional)"
            />
          </div>

          <button onClick={handleSubmit} style={{ marginTop: '10px' }}>
            {editMode ? 'Salvează modificările' : 'Salvează timp de lucru'}
          </button>
        </div>
      )}
    </div>
  );
};

export default WorkCalendar;

