import brgyLogo from '../assets/brgy_84_logo.png'
import { useState } from 'react'

const GenerateBarangayIDPage = () => {
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setSuccess(false)
    setIsSubmitting(true)

    try {
      const response = await fetch('/api/documents/generateBarangayID', {
        method: 'POST',
        body: new FormData(event.currentTarget),
      })

      if (!response.ok) {
        throw new Error((await response.text()) || 'Could not generate the Barangay ID.')
      }

      const pdf = await response.blob()
      const downloadUrl = URL.createObjectURL(pdf)
      const downloadLink = document.createElement('a')
      downloadLink.href = downloadUrl
      downloadLink.download = 'barangay-id.pdf'
      downloadLink.click()
      window.setTimeout(() => URL.revokeObjectURL(downloadUrl), 1000)
      setSuccess(true)
    } catch (requestError) {
      setError(requestError.message || 'Could not generate the Barangay ID.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="bg-gray-300 w-full min-h-dvh flex flex-col">
      <nav className="bg-white text-gray-900 px-4 py-3 sm:p-5 shadow-md flex flex-row items-center gap-3 shrink-0">
        <img src={brgyLogo} className="h-12 w-12 sm:h-16 sm:w-16" alt="Barangay 84 logo" />
        <div className="font-bold text-lg sm:text-2xl">Barangay Management System</div>
      </nav>

      <main className="flex-1 flex p-3 sm:p-5">
        <section className="flex-1 w-full p-4 sm:p-6 lg:p-8 bg-white shadow-md rounded-md flex flex-col">
          <div className="my-auto mx-auto w-full max-w-4xl">
            <h1 className="text-xl sm:text-2xl font-bold mb-2">Generate Barangay ID</h1>
            <p className="text-gray-600 mb-6">Enter the resident and emergency contact details.</p>

            <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4 sm:gap-5">
              <label className="flex flex-col gap-1.5 md:col-span-2">
                <span className="font-medium text-gray-800">Full name</span>
                <input
                  name="Name"
                  type="text"
                  required
                  maxLength={150}
                  placeholder="Last, First Middle Initial"
                  className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 uppercase focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                />
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-medium text-gray-800">Civil status</span>
                <select
                  name="CivilStatus"
                  required
                  defaultValue=""
                  className="w-full min-w-0 rounded border border-gray-400 bg-white px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                >
                  <option value="" disabled>Select civil status</option>
                  <option>Single</option>
                  <option>Married</option>
                  <option>Widowed</option>
                  <option>Separated</option>
                  <option>Divorced</option>
                </select>
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-medium text-gray-800">Gender</span>
                <select
                  name="Gender"
                  required
                  defaultValue=""
                  className="w-full min-w-0 rounded border border-gray-400 bg-white px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                >
                  <option value="" disabled>Select gender</option>
                  <option>Female</option>
                  <option>Male</option>
                  <option>Other</option>
                </select>
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-medium text-gray-800">Date of birth</span>
                <input
                  name="DateOfBirth"
                  type="date"
                  required
                  className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                />
              </label>

              <label className="flex flex-col gap-1.5 md:col-span-2">
                <span className="font-medium text-gray-800">ID photo</span>
                <input
                  name="Photo"
                  type="file"
                  accept="image/png,image/jpeg"
                  required
                  className="w-full min-w-0 rounded border border-gray-400 bg-white px-3 py-2 file:mr-3 file:rounded file:border-0 file:bg-gray-100 file:px-3 file:py-1.5 file:font-medium"
                />
                <span className="text-sm text-gray-600">PNG or JPEG, up to 5 MB.</span>
              </label>

              <label className="flex flex-col gap-1.5 md:col-span-2">
                <span className="font-medium text-gray-800">Home address</span>
                <input
                  name="HomeAddress"
                  type="text"
                  required
                  maxLength={250}
                  className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 uppercase focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                />
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-medium text-gray-800">Emergency contact person</span>
                <input
                  name="ContactPerson"
                  type="text"
                  required
                  maxLength={150}
                  className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                />
              </label>

              <label className="flex flex-col gap-1.5">
                <span className="font-medium text-gray-800">Emergency contact number</span>
                <input
                  name="ContactNumber"
                  type="tel"
                  required
                  maxLength={30}
                  className="w-full min-w-0 rounded border border-gray-400 px-3 py-2 focus:border-blue-700 focus:outline-none focus:ring-1 focus:ring-blue-700"
                />
              </label>

              <div className="md:col-span-2 flex flex-wrap items-center gap-4 pt-2">
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full sm:w-auto rounded bg-blue-800 px-5 py-2.5 font-semibold text-white hover:bg-blue-900 disabled:cursor-wait disabled:opacity-60"
                >
                  {isSubmitting ? 'Generating…' : 'Generate and download PDF'}
                </button>
                <p role="status" aria-live="polite" className="text-sm text-green-800">
                  {success ? 'Barangay ID downloaded.' : ''}
                </p>
                {error && <p role="alert" className="text-sm text-red-700">{error}</p>}
              </div>
            </form>
          </div>
        </section>
      </main>
    </div>
  )
}

export default GenerateBarangayIDPage;