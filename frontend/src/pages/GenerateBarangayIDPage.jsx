import brgyLogo from '../assets/brgy_84_logo.png'
import { useState } from 'react'
import GenerateBarangayIDForm from '../components/GenerateBarangayIDForm'
import GenerateIndigencyForm from '../components/GenerateIndigencyForm'

const GenerateBarangayIDPage = () => {
  const [selectedDocument, setSelectedDocument] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)

  const handleBackToChoices = () => {
    setSelectedDocument('')
    setError('')
    setSuccess(false)
  }

  const handleSubmit = async (formData) => {
    setError('')
    setSuccess(false)
    setIsSubmitting(true)

    try {
      const isBarangayId = selectedDocument === 'barangay-id'
      const response = await fetch(
        isBarangayId
          ? '/api/documents/generateBarangayID'
          : '/api/documents/generateIndigency',
        {
        method: 'POST',
        body: formData,
        },
      )

      if (!response.ok) {
        throw new Error((await response.text()) || 'Could not generate the document.')
      }

      const pdf = await response.blob()
      const downloadUrl = URL.createObjectURL(pdf)
      const downloadLink = document.createElement('a')
      downloadLink.href = downloadUrl
      downloadLink.download = isBarangayId ? 'barangay-id.pdf' : 'indigency.pdf'
      downloadLink.click()
      window.setTimeout(() => URL.revokeObjectURL(downloadUrl), 1000)
      setSuccess(true)
    } catch (requestError) {
      setError(requestError.message || 'Could not generate the document.')
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
          {selectedDocument && (
            <button
              type="button"
              onClick={handleBackToChoices}
              className="inline-flex self-start mb-6 items-center gap-2 rounded border border-gray-300 px-3 py-2 text-sm font-medium text-gray-800 hover:bg-gray-100"
            >
              <span aria-hidden="true" className="text-lg leading-none">←</span>
              Back to document choices
            </button>
          )}
          <div className="my-auto mx-auto w-full max-w-4xl">
            <h1 className="text-xl sm:text-2xl font-bold mb-2">
              {selectedDocument ? 'Request a document' : 'What document would you like to request?'}
            </h1>
            <p className="text-gray-600 mb-6">
              {selectedDocument
                ? 'Enter the required details to generate your document.'
                : 'Choose a document type to continue.'}
            </p>

            {!selectedDocument ? (
              <fieldset className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <legend className="sr-only">Document type</legend>
                <label className="flex cursor-pointer items-start gap-3 rounded border border-gray-300 p-4 hover:border-blue-700 has-[:checked]:border-blue-800 has-[:checked]:bg-blue-50">
                  <input
                    type="radio"
                    name="documentType"
                    value="barangay-id"
                    checked={selectedDocument === 'barangay-id'}
                    onChange={() => setSelectedDocument('barangay-id')}
                    className="mt-1 accent-blue-800"
                  />
                  <span>
                    <span className="block font-semibold text-gray-900">Barangay ID</span>
                    <span className="mt-1 block text-sm text-gray-600">Request a resident identification card.</span>
                  </span>
                </label>
                <label className="flex cursor-pointer items-start gap-3 rounded border border-gray-300 p-4 hover:border-blue-700 has-[:checked]:border-blue-800 has-[:checked]:bg-blue-50">
                  <input
                    type="radio"
                    name="documentType"
                    value="indigency"
                    checked={selectedDocument === 'indigency'}
                    onChange={() => setSelectedDocument('indigency')}
                    className="mt-1 accent-blue-800"
                  />
                  <span>
                    <span className="block font-semibold text-gray-900">Certificate of Indigency</span>
                    <span className="mt-1 block text-sm text-gray-600">Request a certificate for your stated purpose.</span>
                  </span>
                </label>
              </fieldset>
            ) : (
              <>
                {selectedDocument === 'barangay-id' ? (
                  <GenerateBarangayIDForm
                    onSubmit={handleSubmit}
                    isSubmitting={isSubmitting}
                    error={error}
                    success={success}
                    successMessage="Barangay ID downloaded."
                  />
                ) : (
                  <GenerateIndigencyForm
                    onSubmit={handleSubmit}
                    isSubmitting={isSubmitting}
                    error={error}
                    success={success}
                  />
                )}
              </>
            )}
          </div>
        </section>
      </main>
    </div>
  )
}

export default GenerateBarangayIDPage;