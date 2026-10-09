const GenerateBarangayIDForm = ({ onSubmit, isSubmitting, error, success, successMessage }) => {
  const handleSubmit = (event) => {
    event.preventDefault()
    onSubmit(new FormData(event.currentTarget))
  }

  return (
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
          {success ? successMessage : ''}
        </p>
        {error && <p role="alert" className="text-sm text-red-700">{error}</p>}
      </div>
    </form>
  )
}

export default GenerateBarangayIDForm