import './App.css'
import { BrowserRouter, Routes, Route } from 'react-router-dom'

import GenerateBarangayIDPage from './pages/GenerateBarangayIDPage'

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<GenerateBarangayIDPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
